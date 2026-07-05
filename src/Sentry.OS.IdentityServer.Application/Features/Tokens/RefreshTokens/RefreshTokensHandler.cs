using MediatR;
using Microsoft.Extensions.Logging;
using Sentry.OS.Domain.Tokens;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;
using Sentry.OS.IdentityServer.Application.Common.Security;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.RefreshTokens;

/// <summary>
/// Renews tokens from a refresh token, rotating it (FR-009): a superseded (already consumed or
/// revoked) token presented again is treated as compromise — the entire forward lineage is revoked
/// and no tokens are issued.
/// </summary>
public class RefreshTokensHandler(
    IRefreshTokenRepository refreshTokens,
    IAuthClientRepository clients,
    IAuthUserRepository users,
    ScopeIntersectionResolver scopeIntersectionResolver,
    IJwtTokenService jwtTokenService,
    IIdentityServerOptions options,
    TimeProvider timeProvider,
    ILogger<RefreshTokensHandler> logger)
    : IRequestHandler<RefreshTokensCommand, RefreshTokensResponse>
{
    public async Task<RefreshTokensResponse> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var tokenHash = TokenHashing.Hash(request.RefreshToken);

        var existing = await refreshTokens.FindByTokenHashAsync(tokenHash, cancellationToken);
        if (existing is null)
        {
            return Fail("invalid_grant", "The refresh token is invalid.");
        }

        if (existing.ConsumedAtUtc is not null || existing.RevokedAtUtc is not null)
        {
            await RevokeLineageAsync(existing.Id, RefreshTokenRevocationReason.ReuseDetected, now, cancellationToken);
            logger.LogWarning("Refresh token reuse detected for user {UserId}; lineage revoked", existing.UserId);
            return Fail("invalid_grant", "The refresh token is invalid.");
        }

        if (existing.ExpiresAtUtc <= now.UtcDateTime)
        {
            return Fail("invalid_grant", "The refresh token has expired.");
        }

        var client = await clients.FindByClientIdAsync(request.ClientId, cancellationToken);
        if (client is null || !client.IsActive || client.Id != existing.ClientId)
        {
            return Fail("invalid_client", "The client is unknown or inactive.");
        }

        var user = await users.FindByIdAsync(existing.UserId, cancellationToken);
        if (user is null || user.IsDisabled)
        {
            return Fail("invalid_grant", "The user is no longer eligible to sign in.");
        }

        var userScopes = await users.GetGrantedScopeNamesAsync(existing.UserId, existing.OrganizationId, cancellationToken);
        var clientScopes = client.AllowedScopes.Select(a => a.Scope.Name).ToList();
        var grantedScopes = scopeIntersectionResolver.Resolve(userScopes, clientScopes, clientScopes);

        var roleNames = await users.GetAssignedRoleNamesAsync(existing.UserId, existing.OrganizationId, cancellationToken);
        var roleLevels = await users.GetAdministrativeRoleLevelsAsync(existing.UserId, existing.OrganizationId, cancellationToken);

        var accessToken = jwtTokenService.CreateAccessToken(
            user.Id, client.ClientId, existing.OrganizationId, user.IsGlobalAdministrator,
            roleNames, roleLevels, grantedScopes, options.DefaultAudience, client.AccessTokenLifetimeSeconds);

        var name = $"{user.FirstName} {user.LastName}".Trim();
        var identityToken = jwtTokenService.CreateIdentityToken(
            user.Id, client.ClientId, string.IsNullOrWhiteSpace(name) ? null : name, user.Email, user.EmailVerified,
            now, nonce: null, client.IdentityTokenLifetimeSeconds);

        string rawRefreshToken;
        if (client.RefreshTokenRotationEnabled)
        {
            var newTokenId = Guid.NewGuid();
            rawRefreshToken = OneTimeCodeGenerator.GenerateUrlSafeToken();

            existing.ConsumedAtUtc = now.UtcDateTime;
            existing.RevocationReason = RefreshTokenRevocationReason.Rotated;
            existing.ReplacedByTokenId = newTokenId;

            refreshTokens.Add(new RefreshToken
            {
                Id = newTokenId,
                OrganizationId = existing.OrganizationId,
                UserId = existing.UserId,
                ClientId = existing.ClientId,
                TokenHash = TokenHashing.Hash(rawRefreshToken),
                CreatedAtUtc = now.UtcDateTime,
                ExpiresAtUtc = now.UtcDateTime.AddSeconds(client.RefreshTokenLifetimeSeconds)
            });
        }
        else
        {
            rawRefreshToken = request.RefreshToken;
        }

        await refreshTokens.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Access token renewed for account {UserId} via client {ClientId}", user.Id, client.ClientId);

        return new RefreshTokensResponse(
            true, null, null, accessToken.Token, identityToken.Token, rawRefreshToken,
            client.AccessTokenLifetimeSeconds, grantedScopes.Count > 0 ? string.Join(' ', grantedScopes) : null);
    }

    private async Task RevokeLineageAsync(Guid tokenId, RefreshTokenRevocationReason reason, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var lineage = await refreshTokens.GetForwardLineageAsync(tokenId, cancellationToken);
        foreach (var token in lineage.Where(t => t.RevokedAtUtc is null))
        {
            token.RevokedAtUtc = now.UtcDateTime;
            token.RevocationReason = reason;
        }

        await refreshTokens.SaveChangesAsync(cancellationToken);
    }

    private static RefreshTokensResponse Fail(string errorCode, string errorDescription) =>
        new(false, errorCode, errorDescription, null, null, null, 0, null);
}
