using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Persistence.Repositories;

public class RoleRepository(IdentityDbContext dbContext) : IRoleRepository
{
    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Roles
            .Include(r => r.RoleScopes).ThenInclude(rs => rs.Scope)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<Role?> GetByIdWithUsageAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Roles
            .Include(r => r.RoleAssignments)
            .Include(r => r.RoleScopes)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<bool> NameExistsAsync(Guid organizationId, string name, CancellationToken cancellationToken) =>
        dbContext.Roles.AnyAsync(r => r.OrganizationId == organizationId && r.Name == name, cancellationToken);

    public async Task<(IReadOnlyList<Role> Items, int TotalCount)> ListAsync(
        Guid organizationId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Roles
            .Include(r => r.RoleScopes).ThenInclude(rs => rs.Scope)
            .Where(r => r.OrganizationId == organizationId)
            .OrderBy(r => r.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Role role) => dbContext.Roles.Add(role);

    public void Remove(Role role) => dbContext.Roles.Remove(role);

    public Task<bool> ScopeExistsAsync(Guid scopeId, CancellationToken cancellationToken) =>
        dbContext.Scopes.AnyAsync(s => s.Id == scopeId, cancellationToken);

    public Task<RoleScope?> GetRoleScopeAsync(Guid roleId, Guid scopeId, CancellationToken cancellationToken) =>
        dbContext.RoleScopes.FirstOrDefaultAsync(rs => rs.RoleId == roleId && rs.ScopeId == scopeId, cancellationToken);

    public void AddRoleScope(RoleScope roleScope) => dbContext.RoleScopes.Add(roleScope);

    public void RemoveRoleScope(RoleScope roleScope) => dbContext.RoleScopes.Remove(roleScope);

    public void AddRoleAssignment(RoleAssignment assignment) => dbContext.RoleAssignments.Add(assignment);

    public Task<RoleAssignment?> GetRoleAssignmentAsync(Guid userId, Guid roleId, CancellationToken cancellationToken) =>
        dbContext.RoleAssignments
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.UserId == userId && a.RoleId == roleId, cancellationToken);

    public async Task<IReadOnlyList<RoleAssignment>> ListRoleAssignmentsForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.RoleAssignments
            .Include(a => a.Role)
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

    public void RemoveRoleAssignment(RoleAssignment assignment) => dbContext.RoleAssignments.Remove(assignment);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
