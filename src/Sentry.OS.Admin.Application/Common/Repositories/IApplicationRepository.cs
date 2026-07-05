using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="DomainApplication"/>. Carries no business rules.</summary>
public interface IApplicationRepository
{
    /// <summary>Fetches an application scoped by the ambient organization-isolation filter.</summary>
    Task<DomainApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Fetches an application ignoring the organization-isolation filter, so its owning
    /// organization id can be resolved before an explicit access check is performed.
    /// </summary>
    Task<DomainApplication?> GetByIdIgnoringOrganizationAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(Guid organizationId, string slug, CancellationToken cancellationToken);

    Task<(IReadOnlyList<DomainApplication> Items, int TotalCount)> ListAsync(
        Guid organizationId, int page, int pageSize, CancellationToken cancellationToken);

    void Add(DomainApplication application);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
