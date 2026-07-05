using Sentry.OS.Domain.Tokens;

namespace Sentry.OS.IdentityServer.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="RefreshToken"/> (append-only rotation lineage). Carries no business rules.</summary>
public interface IRefreshTokenRepository
{
    /// <summary>Finds a refresh token by the hash of its presented value, ignoring organization scope.</summary>
    Task<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the given token and every descendant reachable by following <see cref="RefreshToken.ReplacedByTokenId"/>
    /// forward — the full forward lineage from a given token to its current tip. Used to revoke an entire
    /// lineage when reuse of a superseded token is detected.
    /// </summary>
    Task<IReadOnlyList<RefreshToken>> GetForwardLineageAsync(Guid refreshTokenId, CancellationToken cancellationToken);

    /// <summary>Registers a new refresh token for insertion.</summary>
    void Add(RefreshToken refreshToken);

    /// <summary>Persists tracked changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
