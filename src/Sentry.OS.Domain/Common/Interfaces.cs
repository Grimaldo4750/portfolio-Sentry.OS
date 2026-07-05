namespace Sentry.OS.Domain.Common;

/// <summary>Marks an entity that is logically deleted rather than physically removed.</summary>
public interface ISoftDelete
{
    /// <summary>When true, the record is excluded from normal queries.</summary>
    bool IsDeleted { get; set; }

    /// <summary>UTC timestamp when the record was soft-deleted.</summary>
    DateTime? DeletedAtUtc { get; set; }
}

/// <summary>Marks an entity that belongs to exactly one organization and is subject to isolation.</summary>
public interface IOrganizationScoped
{
    /// <summary>Owning organization identifier.</summary>
    Guid OrganizationId { get; set; }
}
