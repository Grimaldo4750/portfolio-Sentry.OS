using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.AssignRoleToUser;

public class AssignRoleToUserHandler(
    IUserRepository users,
    IRoleRepository roles,
    ICurrentActor currentActor,
    TimeProvider timeProvider)
    : IRequestHandler<AssignRoleToUserCommand, AssignRoleToUserResponse>
{
    public async Task<AssignRoleToUserResponse> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        if (await users.GetInOrganizationAsync(request.OrganizationId, request.UserId, cancellationToken) is null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        var role = await roles.GetByIdAsync(request.RoleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Role), request.RoleId);

        if (role.Level.HasValue && !currentActor.IsGlobalAdministrator && role.Level >= currentActor.HighestRoleLevel)
        {
            throw new ForbiddenException("You cannot assign a role at or above your own highest role level.");
        }

        var assignment = new RoleAssignment
        {
            OrganizationId = request.OrganizationId,
            UserId = request.UserId,
            RoleId = request.RoleId,
            AssignedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };

        roles.AddRoleAssignment(assignment);
        await roles.SaveChangesAsync(cancellationToken);

        var response = assignment.Adapt<AssignRoleToUserResponse>();
        response.RoleName = role.Name;
        return response;
    }
}
