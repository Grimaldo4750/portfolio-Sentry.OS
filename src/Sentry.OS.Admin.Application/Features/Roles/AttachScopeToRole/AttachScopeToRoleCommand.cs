using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.AttachScopeToRole;

public class AttachScopeToRoleCommand : IRequest<AttachScopeToRoleResponse>, IAuditableRequest
{
    public Guid OrganizationId { get; set; }

    public Guid RoleId { get; set; }

    public Guid ScopeId { get; set; }

    public string AuditAction => "Role.ScopeAttached";

    public string AuditTargetType => nameof(Role);
}
