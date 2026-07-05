using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.IdentityServer.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="User"/> and its organization/role data needed for authentication. Carries no business rules.</summary>
public interface IAuthUserRepository
{
    /// <summary>Finds a user by normalized (upper-invariant) email, ignoring organization scope (users are platform-global).</summary>
    Task<User?> FindByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    /// <summary>Finds a user by id.</summary>
    Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>Finds the user's home organization membership.</summary>
    Task<OrganizationMembership?> FindHomeMembershipAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>Finds the user's membership in a specific organization, if any.</summary>
    Task<OrganizationMembership?> FindMembershipAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken);

    /// <summary>The distinct scope names granted to the user within an organization via RoleAssignment → Role → RoleScope → Scope.</summary>
    Task<IReadOnlyList<string>> GetGrantedScopeNamesAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken);

    /// <summary>The distinct role names assigned to the user within an organization.</summary>
    Task<IReadOnlyList<string>> GetAssignedRoleNamesAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken);

    /// <summary>The administrative <c>Level</c> values of the user's assigned roles within an organization (nulls excluded).</summary>
    Task<IReadOnlyList<int>> GetAdministrativeRoleLevelsAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken);

    /// <summary>The user's persistent custom claims (type/value pairs), promoted into the userinfo projection.</summary>
    Task<IReadOnlyList<UserClaim>> GetPersistentClaimsAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>Persists tracked changes to the user (and related entities loaded in the same context).</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
