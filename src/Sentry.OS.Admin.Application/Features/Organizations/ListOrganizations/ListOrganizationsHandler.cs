using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.Organizations.Dtos;

namespace Sentry.OS.Admin.Application.Features.Organizations.ListOrganizations;

public class ListOrganizationsHandler(IOrganizationRepository organizations)
    : IRequestHandler<ListOrganizationsQuery, ListOrganizationsResponse>
{
    public async Task<ListOrganizationsResponse> Handle(ListOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await organizations.ListAsync(request.Page, request.PageSize, cancellationToken);

        return new ListOrganizationsResponse
        {
            Items = items.Adapt<List<OrganizationDto>>(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
