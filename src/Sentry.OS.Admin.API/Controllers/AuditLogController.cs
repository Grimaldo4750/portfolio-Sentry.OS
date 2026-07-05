using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Audit.Dtos;
using Sentry.OS.Admin.Application.Features.Audit.QueryAuditLog;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Controllers;

/// <summary>Read-only access to the organization's administrative audit trail.</summary>
[ApiController]
[Authorize]
[Route("api/organizations/{organizationId:guid}/audit-log")]
public class AuditLogController(IMediator mediator) : ControllerBase
{
    /// <summary>Queries audit log entries, optionally filtered by date range and target type.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AuditLogEntryDto>>>> Query(
        Guid organizationId, [FromQuery] QueryAuditLogQuery query, CancellationToken cancellationToken)
    {
        query.OrganizationId = organizationId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<AuditLogEntryDto>>.Success(result));
    }
}
