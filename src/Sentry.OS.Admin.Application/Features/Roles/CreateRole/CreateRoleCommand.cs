using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.CreateRole;

public class CreateRoleCommand : IRequest<CreateRoleResponse>, IAuditableRequest
{
    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Level { get; set; }

    public string AuditAction => "Role.Created";

    public string AuditTargetType => nameof(Role);
}
