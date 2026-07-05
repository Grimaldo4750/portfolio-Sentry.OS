using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.UpdateRole;

public class UpdateRoleHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<UpdateRoleCommand, UpdateRoleResponse>
{
    public async Task<UpdateRoleResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var role = await roles.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Role), request.Id);

        if (role.Level.HasValue && !currentActor.IsGlobalAdministrator && role.Level >= currentActor.HighestRoleLevel)
        {
            throw new ForbiddenException("You cannot edit a role at or above your own highest role level.");
        }

        if (request.Level.HasValue && !currentActor.IsGlobalAdministrator && request.Level >= currentActor.HighestRoleLevel)
        {
            throw new ForbiddenException("You cannot set a role to a level at or above your own highest role level.");
        }

        role.Name = request.Name;
        role.Description = request.Description;
        role.Level = request.Level;

        await roles.SaveChangesAsync(cancellationToken);

        return role.Adapt<UpdateRoleResponse>();
    }
}
