using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="Client"/> and its allowed scopes. Carries no business rules.</summary>
public interface IClientRepository
{
    /// <summary>Fetches a client (ignoring organization isolation) including its allowed scopes.</summary>
    Task<Client?> GetByIdAsync(Guid applicationId, Guid id, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Client> Items, int TotalCount)> ListAsync(
        Guid applicationId, int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Counts how many of the given scope ids belong to an API resource owned by the application.</summary>
    Task<int> CountScopesBelongingToApplicationAsync(
        Guid applicationId, IReadOnlyCollection<Guid> scopeIds, CancellationToken cancellationToken);

    void Add(Client client);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
