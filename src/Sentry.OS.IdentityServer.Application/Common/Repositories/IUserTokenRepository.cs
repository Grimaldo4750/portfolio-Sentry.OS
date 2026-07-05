using Sentry.OS.Domain.Users;

namespace Sentry.OS.IdentityServer.Application.Common.Repositories;

/// <summary>
/// CRUD access to <see cref="UserToken"/> one-time secrets (email verification, two-factor). Carries
/// no business rules. Shared by email verification (FR-031) and two-factor authentication (FR-032).
/// </summary>
public interface IUserTokenRepository
{
    /// <summary>Finds the most recently created, still-unconsumed token for the user and purpose (expired or not — callers decide).</summary>
    Task<UserToken?> FindActiveByUserAndPurposeAsync(Guid userId, UserTokenPurpose purpose, CancellationToken cancellationToken);

    /// <summary>Finds an unconsumed token by the hash of its presented value and purpose.</summary>
    Task<UserToken?> FindByTokenHashAsync(string tokenHash, UserTokenPurpose purpose, CancellationToken cancellationToken);

    /// <summary>Registers a new one-time token for insertion.</summary>
    void Add(UserToken userToken);

    /// <summary>Persists tracked changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
