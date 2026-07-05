using Sentry.OS.Domain.Common;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Domain.Authorization;

/// <summary>
/// A named role owned directly by an organization (not by an individual application); maps
/// directly to scopes and assigned to users via <see cref="RoleAssignment"/>.
/// </summary>
public class Role : AuditableEntity, IOrganizationScoped
{
    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <summary>Role name, unique within the organization.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional administrative level. Used solely to gate administrative role management (a user
    /// cannot assign, modify, or remove a role whose level is equal to or higher than their own
    /// highest assigned role's level). Never participates in OAuth authorization decisions.
    /// </summary>
    public int? Level { get; set; }

    /// <summary>Owning organization.</summary>
    public Organization Organization { get; set; } = null!;

    /// <summary>Scopes granted by this role.</summary>
    public ICollection<RoleScope> RoleScopes { get; set; } = new List<RoleScope>();

    /// <summary>Users assigned this role.</summary>
    public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
}
