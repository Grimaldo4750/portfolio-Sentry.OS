namespace Sentry.OS.Admin.Application.Common;

/// <summary>
/// Implemented by every mutating command so <c>AuditLoggingBehavior</c> can record it automatically.
/// The response type is expected to expose a public <c>Id</c> property (every DTO in this API does)
/// so the target identifier can be captured without each command repeating that plumbing.
/// </summary>
public interface IAuditableRequest
{
    /// <summary>Short action name recorded on the audit entry (e.g. <c>Organization.Created</c>).</summary>
    string AuditAction { get; }

    /// <summary>Type name of the entity affected (e.g. <c>Organization</c>).</summary>
    string AuditTargetType { get; }
}
