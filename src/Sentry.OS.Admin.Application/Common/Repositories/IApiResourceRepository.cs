using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="ApiResource"/>. Carries no business rules.</summary>
public interface IApiResourceRepository
{
    /// <summary>Fetches an API resource by id only (ignoring organization isolation), no includes.</summary>
    Task<ApiResource?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Fetches an API resource scoped to its application, including its scopes.</summary>
    Task<ApiResource?> GetByIdWithScopesAsync(Guid applicationId, Guid id, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(Guid applicationId, string name, CancellationToken cancellationToken);

    Task<(IReadOnlyList<ApiResource> Items, int TotalCount)> ListAsync(
        Guid applicationId, int page, int pageSize, CancellationToken cancellationToken);

    void Add(ApiResource resource);

    void Remove(ApiResource resource);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
