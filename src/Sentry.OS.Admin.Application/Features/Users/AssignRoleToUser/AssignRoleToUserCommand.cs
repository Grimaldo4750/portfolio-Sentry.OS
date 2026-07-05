using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Users.AssignRoleToUser;

public class AssignRoleToUserCommand : IRequest<AssignRoleToUserResponse>, IAuditableRequest
{
    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public string AuditAction => "RoleAssignment.Created";

    public string AuditTargetType => nameof(RoleAssignment);
}
