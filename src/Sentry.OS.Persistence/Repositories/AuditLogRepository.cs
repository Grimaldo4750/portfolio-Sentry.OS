using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Auditing;

namespace Sentry.OS.Persistence.Repositories;

public class AuditLogRepository(IdentityDbContext dbContext) : IAuditLogRepository
{
    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> QueryAsync(
        Guid organizationId,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? targetType,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.AuditLogs.Where(a => a.OrganizationId == organizationId);

        if (fromUtc.HasValue)
        {
            query = query.Where(a => a.OccurredAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(a => a.OccurredAtUtc <= toUtc.Value);
        }

        if (!string.IsNullOrWhiteSpace(targetType))
        {
            query = query.Where(a => a.TargetType == targetType);
        }

        query = query.OrderByDescending(a => a.OccurredAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
