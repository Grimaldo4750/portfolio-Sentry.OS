using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Audit.Dtos;

namespace Sentry.OS.Admin.Application.Features.Audit.QueryAuditLog;

public class QueryAuditLogResponse : PagedResult<AuditLogEntryDto>;
