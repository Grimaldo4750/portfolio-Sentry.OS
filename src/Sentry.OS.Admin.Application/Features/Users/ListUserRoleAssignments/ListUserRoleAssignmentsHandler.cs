using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.Users.Dtos;

namespace Sentry.OS.Admin.Application.Features.Users.ListUserRoleAssignments;

public class ListUserRoleAssignmentsHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<ListUserRoleAssignmentsQuery, ListUserRoleAssignmentsResponse>
{
    public async Task<ListUserRoleAssignmentsResponse> Handle(ListUserRoleAssignmentsQuery request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var assignments = await roles.ListRoleAssignmentsForUserAsync(request.UserId, cancellationToken);

        var response = new ListUserRoleAssignmentsResponse();
        response.AddRange(assignments.Adapt<List<RoleAssignmentDto>>());
        return response;
    }
}
