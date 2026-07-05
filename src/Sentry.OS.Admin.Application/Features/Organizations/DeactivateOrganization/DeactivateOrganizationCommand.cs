using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Features.Organizations.DeactivateOrganization;

public record DeactivateOrganizationCommand(Guid Id) : IRequest<DeactivateOrganizationResponse>, IAuditableRequest
{
    public string AuditAction => "Organization.Deactivated";

    public string AuditTargetType => nameof(Organization);
}
