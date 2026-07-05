using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.ApiResources.CreateApiResource;

public class CreateApiResourceCommand : IRequest<CreateApiResourceResponse>, IAuditableRequest
{
    public Guid ApplicationId { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string AuditAction => "ApiResource.Created";

    public string AuditTargetType => nameof(ApiResource);
}
