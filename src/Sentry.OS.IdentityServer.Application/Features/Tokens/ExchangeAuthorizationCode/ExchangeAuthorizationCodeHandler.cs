using MediatR;
using Microsoft.Extensions.Logging;
using Sentry.OS.Domain.Tokens;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;
using Sentry.OS.IdentityServer.Application.Common.Security;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.ExchangeAuthorizationCode;

/// <summary>
/// Consumes a one-time authorization code (single-use — FR-008), re-validates the client/redirect/PKCE
/// against the exact context the code was issued for, computes the (user role scopes) ∩ (client allowed
/// scopes) ∩ (requested) intersection (Principle VI), and issues access, identity, and refresh tokens.
/// </summary>
public class ExchangeAuthorizationCodeHandler(
    IAuthorizationCodeStore authorizationCodes,
    IAuthClientRepository clients,
    IAuthUserRepository users,
    IRefreshTokenRepository refreshTokens,
    ScopeIntersectionResolver scopeIntersectionResolver,
    PkceValidator pkceValidator,
    IJwtTokenService jwtTokenService,
    IIdentityServerOptions options,
    TimeProvider timeProvider,
    ILogger<ExchangeAuthorizationCodeHandler> logger)
    : IRequestHandler<ExchangeAuthorizationCodeCommand, ExchangeAuthorizationCodeResponse>
{
    public async Task<ExchangeAuthorizationCodeResponse> Handle(ExchangeAuthorizationCodeCommand request, CancellationToken cancellationToken)
    {
        if (!authorizationCodes.TryConsume(request.Code, out var data) || data is null)
        {
            logger.LogWarning("Token exchange failed: unknown, expired, or already-used authorization code for client {ClientId}", request.ClientId);
            return Fail("invalid_grant", "The authorization code is invalid or has already been used.");
        }

        var client = await clients.FindByClientIdAsync(request.ClientId, cancellationToken);
        if (client is null || !client.IsActive || client.Id != data.ClientId)
        {
            logger.LogWarning("Token exchange failed: unknown or inactive client {ClientId}", request.ClientId);
            return Fail("invalid_client", "The client is unknown or inactive.");
        }

        if (!string.Equals(request.RedirectUri, data.RedirectUri, StringComparison.Ordinal))
        {
            return Fail("invalid_grant", "The redirect location does not match the original request.");
        }

        if (!pkceValidator.Verify(request.CodeVerifier, data.CodeChallenge, data.CodeChallengeMethod))
        {
            return Fail("invalid_grant", "The PKCE code verifier does not match.");
        }

        var user = await users.FindByIdAsync(data.UserId, cancellationToken);
        if (user is null || user.IsDisabled)
        {
            return Fail("invalid_grant", "The user is no longer eligible to sign in.");
        }

        var userScopes = await users.GetGrantedScopeNamesAsync(data.UserId, data.OrganizationId, cancellationToken);
        var clientScopes = client.AllowedScopes.Select(a => a.Scope.Name).ToList();
        var grantedScopes = scopeIntersectionResolver.Resolve(userScopes, clientScopes, data.RequestedScopes);

        var roleNames = await users.GetAssignedRoleNamesAsync(data.UserId, data.OrganizationId, cancellationToken);
        var roleLevels = await users.GetAdministrativeRoleLevelsAsync(data.UserId, data.OrganizationId, cancellationToken);

        var now = timeProvider.GetUtcNow();

        var accessToken = jwtTokenService.CreateAccessToken(
            user.Id, client.ClientId, data.OrganizationId, user.IsGlobalAdministrator,
            roleNames, roleLevels, grantedScopes, options.DefaultAudience, client.AccessTokenLifetimeSeconds);

        var name = $"{user.FirstName} {user.LastName}".Trim();
        var identityToken = jwtTokenService.CreateIdentityToken(
            user.Id, client.ClientId, string.IsNullOrWhiteSpace(name) ? null : name, user.Email, user.EmailVerified,
            now, data.Nonce, client.IdentityTokenLifetimeSeconds);

        var rawRefreshToken = OneTimeCodeGenerator.GenerateUrlSafeToken();
        refreshTokens.Add(new RefreshToken
        {
            OrganizationId = data.OrganizationId,
            UserId = user.Id,
            ClientId = client.Id,
            TokenHash = TokenHashing.Hash(rawRefreshToken),
            CreatedAtUtc = now.UtcDateTime,
            ExpiresAtUtc = now.UtcDateTime.AddSeconds(client.RefreshTokenLifetimeSeconds)
        });

        await refreshTokens.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Access token issued to account {UserId} via client {ClientId}", user.Id, client.ClientId);

        return new ExchangeAuthorizationCodeResponse(
            true, null, null, accessToken.Token, identityToken.Token, rawRefreshToken,
            client.AccessTokenLifetimeSeconds, grantedScopes.Count > 0 ? string.Join(' ', grantedScopes) : null);
    }

    private static ExchangeAuthorizationCodeResponse Fail(string errorCode, string errorDescription) =>
        new(false, errorCode, errorDescription, null, null, null, 0, null);
}
