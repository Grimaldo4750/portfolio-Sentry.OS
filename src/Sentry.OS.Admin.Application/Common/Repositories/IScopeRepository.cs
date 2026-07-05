using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="Scope"/>. Carries no business rules.</summary>
public interface IScopeRepository
{
    Task<Scope?> GetByIdAsync(Guid apiResourceId, Guid id, CancellationToken cancellationToken);

    /// <summary>Fetches a scope including its role and client usages (needed to check delete-safety).</summary>
    Task<Scope?> GetByIdWithUsageAsync(Guid apiResourceId, Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(Guid apiResourceId, string name, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Scope> Items, int TotalCount)> ListAsync(
        Guid apiResourceId, int page, int pageSize, CancellationToken cancellationToken);

    void Add(Scope scope);

    void Remove(Scope scope);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
