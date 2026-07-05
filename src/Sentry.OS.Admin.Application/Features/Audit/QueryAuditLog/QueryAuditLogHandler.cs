using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.Audit.Dtos;

namespace Sentry.OS.Admin.Application.Features.Audit.QueryAuditLog;

public class QueryAuditLogHandler(IAuditLogRepository auditLogs, ICurrentActor currentActor)
    : IRequestHandler<QueryAuditLogQuery, QueryAuditLogResponse>
{
    public async Task<QueryAuditLogResponse> Handle(QueryAuditLogQuery request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var (items, totalCount) = await auditLogs.QueryAsync(
            request.OrganizationId,
            request.FromUtc,
            request.ToUtc,
            request.TargetType,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new QueryAuditLogResponse
        {
            Items = items.Adapt<List<AuditLogEntryDto>>(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
