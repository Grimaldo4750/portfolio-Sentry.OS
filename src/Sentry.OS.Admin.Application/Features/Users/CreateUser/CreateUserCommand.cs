using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.CreateUser;

public class CreateUserCommand : IRequest<CreateUserResponse>, IAuditableRequest
{
    public Guid OrganizationId { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string AuditAction => "User.Created";

    public string AuditTargetType => nameof(User);
}
