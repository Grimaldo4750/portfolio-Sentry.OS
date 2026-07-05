using Sentry.OS.Domain.Common;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Domain.Authorization;

/// <summary>Assigns a user a role.</summary>
public class RoleAssignment : AuditableEntity, IOrganizationScoped
{
    /// <summary>Assigned user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Assigned role.</summary>
    public Guid RoleId { get; set; }

    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <summary>UTC assignment timestamp.</summary>
    public DateTime AssignedAtUtc { get; set; }

    /// <summary>Navigation to the user.</summary>
    public User User { get; set; } = null!;

    /// <summary>Navigation to the role.</summary>
    public Role Role { get; set; } = null!;
}
