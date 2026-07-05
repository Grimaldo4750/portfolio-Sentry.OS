using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="Role"/> and its scope attachments. Carries no business rules.</summary>
public interface IRoleRepository
{
    /// <summary>Fetches a role including its attached scopes (needed for scope-name projection).</summary>
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Fetches a role including role assignments and role scopes (needed to check delete-safety).</summary>
    Task<Role?> GetByIdWithUsageAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(Guid organizationId, string name, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Role> Items, int TotalCount)> ListAsync(
        Guid organizationId, int page, int pageSize, CancellationToken cancellationToken);

    void Add(Role role);

    void Remove(Role role);

    Task<bool> ScopeExistsAsync(Guid scopeId, CancellationToken cancellationToken);

    Task<RoleScope?> GetRoleScopeAsync(Guid roleId, Guid scopeId, CancellationToken cancellationToken);

    void AddRoleScope(RoleScope roleScope);

    void RemoveRoleScope(RoleScope roleScope);

    void AddRoleAssignment(RoleAssignment assignment);

    /// <summary>Fetches a role assignment including its role (needed for the role-level constraint).</summary>
    Task<RoleAssignment?> GetRoleAssignmentAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);

    Task<IReadOnlyList<RoleAssignment>> ListRoleAssignmentsForUserAsync(Guid userId, CancellationToken cancellationToken);

    void RemoveRoleAssignment(RoleAssignment assignment);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
