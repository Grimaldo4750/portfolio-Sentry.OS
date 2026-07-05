using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.ApiResources.GetApiResourceById;

public class GetApiResourceByIdHandler(IApiResourceRepository apiResources, ICurrentActor currentActor)
    : IRequestHandler<GetApiResourceByIdQuery, GetApiResourceByIdResponse>
{
    public async Task<GetApiResourceByIdResponse> Handle(GetApiResourceByIdQuery request, CancellationToken cancellationToken)
    {
        var resource = await apiResources.GetByIdWithScopesAsync(request.ApplicationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ApiResource), request.Id);

        currentActor.EnsureOrganizationAccess(resource.OrganizationId);

        return resource.Adapt<GetApiResourceByIdResponse>();
    }
}
