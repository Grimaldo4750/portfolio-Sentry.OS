using Microsoft.EntityFrameworkCore;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Users;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAuthUserRepository"/>. The IdP has no ambient organization
/// (<see cref="IdentityDbContext.CurrentOrganizationId"/> is always null before a user is
/// authenticated), so every organization-scoped query below explicitly filters by the caller-supplied
/// organization id with <c>IgnoreQueryFilters()</c> rather than relying on the ambient query filter.
/// </summary>
public class AuthUserRepository(IdentityDbContext dbContext) : IAuthUserRepository
{
    public Task<User?> FindByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<OrganizationMembership?> FindHomeMembershipAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.OrganizationMemberships
            .IgnoreQueryFilters()
            .Include(m => m.Organization)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.IsHomeOrganization, cancellationToken);

    public Task<OrganizationMembership?> FindMembershipAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken) =>
        dbContext.OrganizationMemberships
            .IgnoreQueryFilters()
            .Include(m => m.Organization)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.OrganizationId == organizationId, cancellationToken);

    public async Task<IReadOnlyList<string>> GetGrantedScopeNamesAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken) =>
        await dbContext.RoleAssignments
            .IgnoreQueryFilters()
            .Where(ra => ra.UserId == userId && ra.OrganizationId == organizationId)
            .SelectMany(ra => ra.Role.RoleScopes.Select(rs => rs.Scope.Name))
            .Distinct()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<string>> GetAssignedRoleNamesAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken) =>
        await dbContext.RoleAssignments
            .IgnoreQueryFilters()
            .Where(ra => ra.UserId == userId && ra.OrganizationId == organizationId)
            .Select(ra => ra.Role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<int>> GetAdministrativeRoleLevelsAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken) =>
        await dbContext.RoleAssignments
            .IgnoreQueryFilters()
            .Where(ra => ra.UserId == userId && ra.OrganizationId == organizationId && ra.Role.Level != null)
            .Select(ra => ra.Role.Level!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<UserClaim>> GetPersistentClaimsAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.UserClaims.Where(c => c.UserId == userId).ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
