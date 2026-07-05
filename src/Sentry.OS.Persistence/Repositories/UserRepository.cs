using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Persistence.Repositories;

public class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public Task<User?> GetInOrganizationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.Users
            .Where(u => u.Id == userId)
            .Where(u => u.Memberships.Any(m => m.OrganizationId == organizationId))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.UserName == userName, cancellationToken);

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> ListInOrganizationAsync(
        Guid organizationId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Users
            .Where(u => u.Memberships.Any(m => m.OrganizationId == organizationId))
            .OrderBy(u => u.Email);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(User user, OrganizationMembership membership)
    {
        dbContext.Users.Add(user);
        dbContext.OrganizationMemberships.Add(membership);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
