using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Roles.AttachScopeToRole;

public class AttachScopeToRoleHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<AttachScopeToRoleCommand, AttachScopeToRoleResponse>
{
    public async Task<AttachScopeToRoleResponse> Handle(AttachScopeToRoleCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var role = await roles.GetByIdAsync(request.RoleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Role), request.RoleId);

        if (!await roles.ScopeExistsAsync(request.ScopeId, cancellationToken))
        {
            throw new NotFoundException(nameof(Scope), request.ScopeId);
        }

        if (role.RoleScopes.All(rs => rs.ScopeId != request.ScopeId))
        {
            roles.AddRoleScope(new RoleScope { RoleId = role.Id, ScopeId = request.ScopeId });
            await roles.SaveChangesAsync(cancellationToken);
        }

        var updated = await roles.GetByIdAsync(role.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Role), role.Id);

        return updated.Adapt<AttachScopeToRoleResponse>();
    }
}
