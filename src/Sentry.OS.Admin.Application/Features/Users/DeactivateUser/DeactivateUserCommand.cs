using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.DeactivateUser;

public record DeactivateUserCommand(Guid OrganizationId, Guid Id) : IRequest<DeactivateUserResponse>, IAuditableRequest
{
    public string AuditAction => "User.Deactivated";

    public string AuditTargetType => nameof(User);
}
