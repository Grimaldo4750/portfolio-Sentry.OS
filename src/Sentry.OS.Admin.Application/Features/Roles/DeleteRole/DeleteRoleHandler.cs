using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.DeleteRole;

public class DeleteRoleHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<DeleteRoleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var role = await roles.GetByIdWithUsageAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Role), request.Id);

        if (role.RoleAssignments.Count != 0 || role.RoleScopes.Count != 0)
        {
            throw new ConflictException("This role is still assigned to users or scopes and cannot be deleted.");
        }

        roles.Remove(role);
        await roles.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
