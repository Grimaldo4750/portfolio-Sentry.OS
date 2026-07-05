using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.DetachScopeFromRole;

public record DetachScopeFromRoleCommand(Guid OrganizationId, Guid RoleId, Guid ScopeId) : IRequest<Unit>, IAuditableRequest
{
    public string AuditAction => "Role.ScopeDetached";

    public string AuditTargetType => nameof(Role);
}
