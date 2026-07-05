using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.ApiResources.Dtos;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.ApiResources.ListApiResources;

public class ListApiResourcesHandler(IApiResourceRepository apiResources, IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<ListApiResourcesQuery, ListApiResourcesResponse>
{
    public async Task<ListApiResourcesResponse> Handle(ListApiResourcesQuery request, CancellationToken cancellationToken)
    {
        var application = await applications.GetByIdIgnoringOrganizationAsync(request.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainApplication), request.ApplicationId);

        currentActor.EnsureOrganizationAccess(application.OrganizationId);

        var (items, totalCount) = await apiResources.ListAsync(
            request.ApplicationId, request.Page, request.PageSize, cancellationToken);

        return new ListApiResourcesResponse
        {
            Items = items.Adapt<List<ApiResourceDto>>(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
