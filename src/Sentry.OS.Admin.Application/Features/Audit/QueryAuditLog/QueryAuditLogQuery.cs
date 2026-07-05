using MediatR;
using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.Application.Features.Audit.QueryAuditLog;

public class QueryAuditLogQuery : PagingRequest, IRequest<QueryAuditLogResponse>
{
    public Guid OrganizationId { get; set; }

    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }

    public string? TargetType { get; set; }
}
