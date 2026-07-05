using Sentry.OS.Domain.Applications;
using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Common;

namespace Sentry.OS.Domain.Organizations;

/// <summary>The sole top-level isolation boundary and root of the authorization structure.</summary>
public class Organization : AuditableEntity, ISoftDelete
{
    /// <summary>Human-readable organization name.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Globally-unique, URL-safe organization key (e.g. <c>sentry</c>).</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Display name shown in UIs.</summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>Whether the organization is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTime? DeletedAtUtc { get; set; }

    /// <summary>Applications owned by this organization.</summary>
    public ICollection<Application> Applications { get; set; } = new List<Application>();

    /// <summary>Roles owned directly by this organization.</summary>
    public ICollection<Role> Roles { get; set; } = new List<Role>();

    /// <summary>Membership links to users that may access this organization.</summary>
    public ICollection<OrganizationMembership> Memberships { get; set; } = new List<OrganizationMembership>();
}
