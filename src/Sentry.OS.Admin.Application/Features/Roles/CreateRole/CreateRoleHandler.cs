using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.CreateRole;

public class CreateRoleHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<CreateRoleCommand, CreateRoleResponse>
{
    public async Task<CreateRoleResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        if (request.Level.HasValue && !currentActor.IsGlobalAdministrator && request.Level >= currentActor.HighestRoleLevel)
        {
            throw new ForbiddenException("You cannot create a role at or above your own highest role level.");
        }

        if (await roles.NameExistsAsync(request.OrganizationId, request.Name, cancellationToken))
        {
            throw new ConflictException("A role with this name already exists in the organization.");
        }

        var role = new Role
        {
            OrganizationId = request.OrganizationId,
            Name = request.Name,
            Description = request.Description,
            Level = request.Level
        };

        roles.Add(role);
        await roles.SaveChangesAsync(cancellationToken);

        return role.Adapt<CreateRoleResponse>();
    }
}
