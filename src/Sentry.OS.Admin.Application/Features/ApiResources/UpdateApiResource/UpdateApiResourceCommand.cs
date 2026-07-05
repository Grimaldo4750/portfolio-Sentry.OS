using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.ApiResources.UpdateApiResource;

public class UpdateApiResourceCommand : IRequest<UpdateApiResourceResponse>, IAuditableRequest
{
    public Guid ApplicationId { get; set; }

    public Guid Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public string AuditAction => "ApiResource.Updated";

    public string AuditTargetType => nameof(ApiResource);
}
