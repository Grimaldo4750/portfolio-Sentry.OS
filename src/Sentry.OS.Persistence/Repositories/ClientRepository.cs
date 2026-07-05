using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Persistence.Repositories;

public class ClientRepository(IdentityDbContext dbContext) : IClientRepository
{
    public Task<Client?> GetByIdAsync(Guid applicationId, Guid id, CancellationToken cancellationToken) =>
        dbContext.Clients.IgnoreQueryFilters()
            .Include(c => c.AllowedScopes).ThenInclude(a => a.Scope)
            .FirstOrDefaultAsync(c => c.Id == id && c.ApplicationId == applicationId, cancellationToken);

    public async Task<(IReadOnlyList<Client> Items, int TotalCount)> ListAsync(
        Guid applicationId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Clients.IgnoreQueryFilters()
            .Include(c => c.AllowedScopes).ThenInclude(a => a.Scope)
            .Where(c => c.ApplicationId == applicationId)
            .OrderBy(c => c.DisplayName);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<int> CountScopesBelongingToApplicationAsync(
        Guid applicationId, IReadOnlyCollection<Guid> scopeIds, CancellationToken cancellationToken) =>
        dbContext.Scopes.IgnoreQueryFilters()
            .Where(s => scopeIds.Contains(s.Id))
            .Where(s => s.ApiResource.ApplicationId == applicationId)
            .CountAsync(cancellationToken);

    public void Add(Client client) => dbContext.Clients.Add(client);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
