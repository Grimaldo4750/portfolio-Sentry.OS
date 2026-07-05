using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.DetachScopeFromRole;

public class DetachScopeFromRoleHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<DetachScopeFromRoleCommand, Unit>
{
    public async Task<Unit> Handle(DetachScopeFromRoleCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var roleScope = await roles.GetRoleScopeAsync(request.RoleId, request.ScopeId, cancellationToken)
            ?? throw new NotFoundException(nameof(RoleScope), request.ScopeId);

        roles.RemoveRoleScope(roleScope);
        await roles.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
