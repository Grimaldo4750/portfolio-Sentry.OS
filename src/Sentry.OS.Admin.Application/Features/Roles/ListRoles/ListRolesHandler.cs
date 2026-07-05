using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.Roles.Dtos;

namespace Sentry.OS.Admin.Application.Features.Roles.ListRoles;

public class ListRolesHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<ListRolesQuery, ListRolesResponse>
{
    public async Task<ListRolesResponse> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var (items, totalCount) = await roles.ListAsync(request.OrganizationId, request.Page, request.PageSize, cancellationToken);

        return new ListRolesResponse
        {
            Items = items.Adapt<List<RoleDto>>(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
