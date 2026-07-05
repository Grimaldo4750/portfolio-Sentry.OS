using MediatR;
using Sentry.OS.Admin.Application.Common;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Applications.UpdateApplication;

public class UpdateApplicationCommand : IRequest<UpdateApplicationResponse>, IAuditableRequest
{
    public Guid OrganizationId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string AuditAction => "Application.Updated";

    public string AuditTargetType => nameof(DomainApplication);
}
