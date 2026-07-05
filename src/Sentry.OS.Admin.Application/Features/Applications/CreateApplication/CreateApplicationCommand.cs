using MediatR;
using Sentry.OS.Admin.Application.Common;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Applications.CreateApplication;

public class CreateApplicationCommand : IRequest<CreateApplicationResponse>, IAuditableRequest
{
    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string AuditAction => "Application.Created";

    public string AuditTargetType => nameof(DomainApplication);
}
