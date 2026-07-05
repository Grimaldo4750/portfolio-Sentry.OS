using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="Organization"/>. Carries no business rules.</summary>
public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Whether an organization with this slug exists, including soft-deleted rows.</summary>
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Organization> Items, int TotalCount)> ListAsync(
        int page, int pageSize, CancellationToken cancellationToken);

    void Add(Organization organization);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
