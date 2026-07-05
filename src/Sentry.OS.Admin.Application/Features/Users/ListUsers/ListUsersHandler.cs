using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.Users.Dtos;

namespace Sentry.OS.Admin.Application.Features.Users.ListUsers;

public class ListUsersHandler(IUserRepository users, ICurrentActor currentActor)
    : IRequestHandler<ListUsersQuery, ListUsersResponse>
{
    public async Task<ListUsersResponse> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var (items, totalCount) = await users.ListInOrganizationAsync(
            request.OrganizationId, request.Page, request.PageSize, cancellationToken);

        return new ListUsersResponse
        {
            Items = items.Adapt<List<UserDto>>(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
