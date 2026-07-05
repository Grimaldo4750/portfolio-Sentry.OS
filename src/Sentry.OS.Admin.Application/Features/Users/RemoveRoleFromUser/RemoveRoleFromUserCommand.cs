using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Users.RemoveRoleFromUser;

public record RemoveRoleFromUserCommand(Guid OrganizationId, Guid UserId, Guid RoleId) : IRequest<Unit>, IAuditableRequest
{
    public string AuditAction => "RoleAssignment.Removed";

    public string AuditTargetType => nameof(RoleAssignment);
}
