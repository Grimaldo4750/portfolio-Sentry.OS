using Microsoft.EntityFrameworkCore;
using Sentry.OS.Domain.Clients;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAuthClientRepository"/>. Ignores the organization query
/// filter (the IdP has no ambient organization) and returns the client regardless of
/// <see cref="Client.IsActive"/> so handlers can distinguish "unknown" from "known but inactive".
/// </summary>
public class AuthClientRepository(IdentityDbContext dbContext) : IAuthClientRepository
{
    public Task<Client?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken) =>
        dbContext.Clients
            .IgnoreQueryFilters()
            .Include(c => c.RedirectUris)
            .Include(c => c.CorsOrigins)
            .Include(c => c.GrantTypes)
            .Include(c => c.AllowedScopes).ThenInclude(a => a.Scope)
            .FirstOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);
}
