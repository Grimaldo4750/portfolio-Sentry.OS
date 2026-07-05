using Sentry.OS.Domain.Auditing;

namespace Sentry.OS.Admin.Application.Common.Repositories;

/// <summary>Read-only query access to <see cref="AuditLog"/>. Carries no business rules.</summary>
public interface IAuditLogRepository
{
    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> QueryAsync(
        Guid organizationId,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? targetType,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
