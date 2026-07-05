namespace Sentry.OS.Domain.Auditing;

/// <summary>
/// An append-only record of an administrative or security event. Actor and target are stored as
/// denormalized identifiers (no foreign keys) so the trail survives downstream deletions.
/// </summary>
public class AuditLog
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Organization context of the event, or null for platform-level events.</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>Identifier of the actor, if known (denormalized, no FK).</summary>
    public Guid? ActorUserId { get; set; }

    /// <summary>Snapshot of the actor's display (email/name) at the time of the event.</summary>
    public string? ActorDisplay { get; set; }

    /// <summary>Action performed (e.g. <c>User.Created</c>).</summary>
    public string Action { get; set; } = null!;

    /// <summary>Type name of the target entity, if applicable.</summary>
    public string? TargetType { get; set; }

    /// <summary>Identifier of the target entity, if applicable (denormalized, no FK).</summary>
    public Guid? TargetId { get; set; }

    /// <summary>Redacted change payload as JSON (never contains secrets).</summary>
    public string? DataJson { get; set; }

    /// <summary>Originating IP address, if captured.</summary>
    public string? IpAddress { get; set; }

    /// <summary>UTC timestamp when the event occurred.</summary>
    public DateTime OccurredAtUtc { get; set; }
}
