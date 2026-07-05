using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Persistence.Repositories;

public class ApiResourceRepository(IdentityDbContext dbContext) : IApiResourceRepository
{
    public Task<ApiResource?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.ApiResources.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<ApiResource?> GetByIdWithScopesAsync(Guid applicationId, Guid id, CancellationToken cancellationToken) =>
        dbContext.ApiResources.IgnoreQueryFilters()
            .Include(r => r.Scopes)
            .FirstOrDefaultAsync(r => r.Id == id && r.ApplicationId == applicationId, cancellationToken);

    public Task<bool> NameExistsAsync(Guid applicationId, string name, CancellationToken cancellationToken) =>
        dbContext.ApiResources.IgnoreQueryFilters()
            .AnyAsync(r => r.ApplicationId == applicationId && r.Name == name, cancellationToken);

    public async Task<(IReadOnlyList<ApiResource> Items, int TotalCount)> ListAsync(
        Guid applicationId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.ApiResources.IgnoreQueryFilters()
            .Where(r => r.ApplicationId == applicationId)
            .OrderBy(r => r.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(ApiResource resource) => dbContext.ApiResources.Add(resource);

    public void Remove(ApiResource resource) => dbContext.ApiResources.Remove(resource);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
