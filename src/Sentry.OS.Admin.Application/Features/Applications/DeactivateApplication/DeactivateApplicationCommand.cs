using MediatR;
using Sentry.OS.Admin.Application.Common;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Applications.DeactivateApplication;

public record DeactivateApplicationCommand(Guid OrganizationId, Guid Id) : IRequest<DeactivateApplicationResponse>, IAuditableRequest
{
    public string AuditAction => "Application.Deactivated";

    public string AuditTargetType => nameof(DomainApplication);
}
