using MediatR;
using Microsoft.Extensions.Logging;
using Sentry.OS.Domain.Tokens;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.RevokeToken;

/// <summary>
/// RFC 7009 revocation: per spec, always reports success to the caller even if the token is
/// unknown or already revoked (never reveals which case occurred), while its forward lineage is
/// revoked when found.
/// </summary>
public class RevokeTokenHandler(IRefreshTokenRepository refreshTokens, TimeProvider timeProvider, ILogger<RevokeTokenHandler> logger)
    : IRequestHandler<RevokeTokenCommand, RevokeTokenResponse>
{
    public async Task<RevokeTokenResponse> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = TokenHashing.Hash(request.Token);
        var existing = await refreshTokens.FindByTokenHashAsync(tokenHash, cancellationToken);

        if (existing is not null)
        {
            var now = timeProvider.GetUtcNow();
            var lineage = await refreshTokens.GetForwardLineageAsync(existing.Id, cancellationToken);

            foreach (var token in lineage.Where(t => t.RevokedAtUtc is null))
            {
                token.RevokedAtUtc = now.UtcDateTime;
                token.RevocationReason = RefreshTokenRevocationReason.Logout;
            }

            await refreshTokens.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Refresh token lineage revoked for user {UserId} (sign-out)", existing.UserId);
        }

        return new RevokeTokenResponse();
    }
}
