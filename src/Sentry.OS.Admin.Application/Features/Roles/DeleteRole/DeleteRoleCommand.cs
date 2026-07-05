using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.DeleteRole;

public record DeleteRoleCommand(Guid OrganizationId, Guid Id) : IRequest<Unit>, IAuditableRequest
{
    public string AuditAction => "Role.Deleted";

    public string AuditTargetType => nameof(Role);
}
