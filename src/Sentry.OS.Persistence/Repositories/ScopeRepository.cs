using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Persistence.Repositories;

public class ScopeRepository(IdentityDbContext dbContext) : IScopeRepository
{
    public Task<Scope?> GetByIdAsync(Guid apiResourceId, Guid id, CancellationToken cancellationToken) =>
        dbContext.Scopes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id && s.ApiResourceId == apiResourceId, cancellationToken);

    public Task<Scope?> GetByIdWithUsageAsync(Guid apiResourceId, Guid id, CancellationToken cancellationToken) =>
        dbContext.Scopes.IgnoreQueryFilters()
            .Include(s => s.RoleScopes)
            .Include(s => s.ClientAllowedScopes)
            .FirstOrDefaultAsync(s => s.Id == id && s.ApiResourceId == apiResourceId, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Scopes.AnyAsync(s => s.Id == id, cancellationToken);

    public Task<bool> NameExistsAsync(Guid apiResourceId, string name, CancellationToken cancellationToken) =>
        dbContext.Scopes.IgnoreQueryFilters()
            .AnyAsync(s => s.ApiResourceId == apiResourceId && s.Name == name, cancellationToken);

    public async Task<(IReadOnlyList<Scope> Items, int TotalCount)> ListAsync(
        Guid apiResourceId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Scopes.IgnoreQueryFilters()
            .Where(s => s.ApiResourceId == apiResourceId)
            .OrderBy(s => s.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Scope scope) => dbContext.Scopes.Add(scope);

    public void Remove(Scope scope) => dbContext.Scopes.Remove(scope);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
