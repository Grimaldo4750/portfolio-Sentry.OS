using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.ApiResources.DeleteApiResource;

public record DeleteApiResourceCommand(Guid ApplicationId, Guid Id) : IRequest<Unit>, IAuditableRequest
{
    public string AuditAction => "ApiResource.Deleted";

    public string AuditTargetType => nameof(ApiResource);
}
