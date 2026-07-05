using Sentry.OS.Domain.Clients;

namespace Sentry.OS.IdentityServer.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="Client"/> and its child collections needed for authentication. Carries no business rules.</summary>
public interface IAuthClientRepository
{
    /// <summary>
    /// Finds a client by its public <see cref="Client.ClientId"/>, ignoring organization scope and
    /// regardless of <see cref="Client.IsActive"/> — callers decide how to react to an inactive or
    /// missing result (FR-007). Eager-loads redirect URIs, CORS origins, grant types, and allowed scopes.
    /// </summary>
    Task<Client?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken);
}
