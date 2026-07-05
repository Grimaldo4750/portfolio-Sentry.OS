using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Persistence.Repositories;

public class OrganizationRepository(IdentityDbContext dbContext) : IOrganizationRepository
{
    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken) =>
        dbContext.Organizations.IgnoreQueryFilters().AnyAsync(o => o.Slug == slug, cancellationToken);

    public async Task<(IReadOnlyList<Organization> Items, int TotalCount)> ListAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Organizations.OrderBy(o => o.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Organization organization) => dbContext.Organizations.Add(organization);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
