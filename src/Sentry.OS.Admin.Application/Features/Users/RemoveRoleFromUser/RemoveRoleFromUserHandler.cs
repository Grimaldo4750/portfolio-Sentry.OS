using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Users.RemoveRoleFromUser;

public class RemoveRoleFromUserHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<RemoveRoleFromUserCommand, Unit>
{
    public async Task<Unit> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var assignment = await roles.GetRoleAssignmentAsync(request.UserId, request.RoleId, cancellationToken)
            ?? throw new NotFoundException(nameof(RoleAssignment), request.RoleId);

        if (assignment.Role.Level.HasValue &&
            !currentActor.IsGlobalAdministrator &&
            assignment.Role.Level >= currentActor.HighestRoleLevel)
        {
            throw new ForbiddenException("You cannot remove a role at or above your own highest role level.");
        }

        roles.RemoveRoleAssignment(assignment);
        await roles.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
