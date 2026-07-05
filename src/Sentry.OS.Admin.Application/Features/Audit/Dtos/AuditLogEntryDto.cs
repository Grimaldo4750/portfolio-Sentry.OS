namespace Sentry.OS.Admin.Application.Features.Audit.Dtos;

public class AuditLogEntryDto
{
    public Guid Id { get; set; }

    public Guid? OrganizationId { get; set; }

    public Guid? ActorUserId { get; set; }

    public string? ActorDisplay { get; set; }

    public string Action { get; set; } = null!;

    public string? TargetType { get; set; }

    public Guid? TargetId { get; set; }

    public DateTime OccurredAtUtc { get; set; }
}
