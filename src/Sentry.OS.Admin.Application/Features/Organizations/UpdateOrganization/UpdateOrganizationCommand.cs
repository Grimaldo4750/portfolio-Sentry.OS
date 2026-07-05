using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Features.Organizations.UpdateOrganization;

public class UpdateOrganizationCommand : IRequest<UpdateOrganizationResponse>, IAuditableRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string AuditAction => "Organization.Updated";

    public string AuditTargetType => nameof(Organization);
}
