using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.UpdateUser;

public class UpdateUserCommand : IRequest<UpdateUserResponse>, IAuditableRequest
{
    public Guid OrganizationId { get; set; }

    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public string AuditAction => "User.Updated";

    public string AuditTargetType => nameof(User);
}
