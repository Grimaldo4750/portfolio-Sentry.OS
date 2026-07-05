using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Features.Organizations.CreateOrganization;

public class CreateOrganizationCommand : IRequest<CreateOrganizationResponse>, IAuditableRequest
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string AuditAction => "Organization.Created";

    public string AuditTargetType => nameof(Organization);
}
