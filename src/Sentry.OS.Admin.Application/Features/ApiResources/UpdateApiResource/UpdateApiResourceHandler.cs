using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.ApiResources.UpdateApiResource;

public class UpdateApiResourceHandler(IApiResourceRepository apiResources, ICurrentActor currentActor)
    : IRequestHandler<UpdateApiResourceCommand, UpdateApiResourceResponse>
{
    public async Task<UpdateApiResourceResponse> Handle(UpdateApiResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await apiResources.GetByIdWithScopesAsync(request.ApplicationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ApiResource), request.Id);

        currentActor.EnsureOrganizationAccess(resource.OrganizationId);

        resource.DisplayName = request.DisplayName;

        await apiResources.SaveChangesAsync(cancellationToken);

        return resource.Adapt<UpdateApiResourceResponse>();
    }
}
