using Sentry.OS.Domain.Applications;
using Sentry.OS.Domain.Common;

namespace Sentry.OS.Domain.Resources;

/// <summary>A protected API resource (audience) belonging to an application; parent of scopes.</summary>
public class ApiResource : AuditableEntity, IOrganizationScoped
{
    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <summary>Owning application.</summary>
    public Guid ApplicationId { get; set; }

    /// <summary>Resource name / audience (e.g. <c>sentry-admin-api</c>).</summary>
    public string Name { get; set; } = null!;

    /// <summary>Display name.</summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>Whether the resource is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Owning application.</summary>
    public Application Application { get; set; } = null!;

    /// <summary>Scopes belonging to this resource.</summary>
    public ICollection<Scope> Scopes { get; set; } = new List<Scope>();
}
