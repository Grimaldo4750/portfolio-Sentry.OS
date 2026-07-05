using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Auditing;
using Sentry.OS.Persistence;

namespace Sentry.OS.Admin.Infrastructure.Auditing;

/// <summary>
/// Writes an <see cref="AuditLog"/> row for every command that implements <see cref="IAuditableRequest"/>
/// and completes successfully, so mutation coverage does not depend on each handler remembering to log.
/// </summary>
public class AuditLoggingBehavior<TRequest, TResponse>(
    IdentityDbContext dbContext,
    ICurrentActor currentActor,
    TimeProvider timeProvider)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        if (request is IAuditableRequest auditable)
        {
            var targetId = response?.GetType().GetProperty("Id")?.GetValue(response) as Guid?;

            dbContext.AuditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                OrganizationId = currentActor.OrganizationId,
                ActorUserId = currentActor.UserId,
                Action = auditable.AuditAction,
                TargetType = auditable.AuditTargetType,
                TargetId = targetId,
                OccurredAtUtc = timeProvider.GetUtcNow().UtcDateTime
            });

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}
