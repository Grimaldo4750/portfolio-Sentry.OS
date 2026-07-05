using Sentry.OS.Domain.Common;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Domain.Organizations;

/// <summary>
/// Links a platform-global <see cref="User"/> to an <see cref="Organization"/> they may access.
/// Enables single-organization users, global users spanning organizations, organization
/// administrators, and organization switching.
/// </summary>
public class OrganizationMembership : AuditableEntity, IOrganizationScoped
{
    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <summary>The user that holds this membership.</summary>
    public Guid UserId { get; set; }

    /// <summary>Whether the user is an administrator within this organization.</summary>
    public bool IsOrganizationAdministrator { get; set; }

    /// <summary>Whether this is the user's default/home organization used when switching.</summary>
    public bool IsHomeOrganization { get; set; }

    /// <summary>Whether the membership is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>UTC timestamp when the user joined the organization.</summary>
    public DateTime JoinedAtUtc { get; set; }

    /// <summary>Navigation to the owning organization.</summary>
    public Organization Organization { get; set; } = null!;

    /// <summary>Navigation to the member user.</summary>
    public User User { get; set; } = null!;
}
