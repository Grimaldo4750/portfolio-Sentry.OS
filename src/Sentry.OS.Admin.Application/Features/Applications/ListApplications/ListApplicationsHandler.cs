using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.Applications.Dtos;

namespace Sentry.OS.Admin.Application.Features.Applications.ListApplications;

public class ListApplicationsHandler(IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<ListApplicationsQuery, ListApplicationsResponse>
{
    public async Task<ListApplicationsResponse> Handle(ListApplicationsQuery request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var (items, totalCount) = await applications.ListAsync(
            request.OrganizationId, request.Page, request.PageSize, cancellationToken);

        return new ListApplicationsResponse
        {
            Items = items.Adapt<List<ApplicationDto>>(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
