using Sentry.OS.Domain.Clients;
using Sentry.OS.Domain.Common;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Domain.Applications;

/// <summary>A software application owned by an organization; parent of clients and resources.</summary>
public class Application : AuditableEntity, ISoftDelete, IOrganizationScoped
{
    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <summary>Application name.</summary>
    public string Name { get; set; } = null!;

    /// <summary>URL-safe key, unique within the organization.</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>Whether the application is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTime? DeletedAtUtc { get; set; }

    /// <summary>Owning organization.</summary>
    public Organization Organization { get; set; } = null!;

    /// <summary>Clients belonging to this application.</summary>
    public ICollection<Client> Clients { get; set; } = new List<Client>();

    /// <summary>API resources belonging to this application.</summary>
    public ICollection<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
}
