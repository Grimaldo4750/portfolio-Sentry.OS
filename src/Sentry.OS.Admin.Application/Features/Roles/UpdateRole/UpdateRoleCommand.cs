using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.UpdateRole;

public class UpdateRoleCommand : IRequest<UpdateRoleResponse>, IAuditableRequest
{
    public Guid OrganizationId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Level { get; set; }

    public string AuditAction => "Role.Updated";

    public string AuditTargetType => nameof(Role);
}
