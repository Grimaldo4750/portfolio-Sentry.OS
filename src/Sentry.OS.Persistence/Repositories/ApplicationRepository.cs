using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common.Repositories;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Persistence.Repositories;

public class ApplicationRepository(IdentityDbContext dbContext) : IApplicationRepository
{
    public Task<DomainApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<DomainApplication?> GetByIdIgnoringOrganizationAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Applications.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<bool> SlugExistsAsync(Guid organizationId, string slug, CancellationToken cancellationToken) =>
        dbContext.Applications.AnyAsync(a => a.OrganizationId == organizationId && a.Slug == slug, cancellationToken);

    public async Task<(IReadOnlyList<DomainApplication> Items, int TotalCount)> ListAsync(
        Guid organizationId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Applications
            .Where(a => a.OrganizationId == organizationId)
            .OrderBy(a => a.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(DomainApplication application) => dbContext.Applications.Add(application);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
