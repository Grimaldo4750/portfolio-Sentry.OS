using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Clients;
using Sentry.OS.Domain.Common;

namespace Sentry.OS.Domain.Resources;

/// <summary>A permission unit belonging to exactly one API resource (e.g. <c>admin.read</c>).</summary>
public class Scope : AuditableEntity, IOrganizationScoped
{
    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <summary>Owning API resource.</summary>
    public Guid ApiResourceId { get; set; }

    /// <summary>Scope name, unique within the resource.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Display name.</summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>Owning API resource.</summary>
    public ApiResource ApiResource { get; set; } = null!;

    /// <summary>Roles that grant this scope.</summary>
    public ICollection<RoleScope> RoleScopes { get; set; } = new List<RoleScope>();

    /// <summary>Clients allowed to request this scope.</summary>
    public ICollection<ClientAllowedScope> ClientAllowedScopes { get; set; } = new List<ClientAllowedScope>();
}
