using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.GetRoleById;

public class GetRoleByIdHandler(IRoleRepository roles, ICurrentActor currentActor)
    : IRequestHandler<GetRoleByIdQuery, GetRoleByIdResponse>
{
    public async Task<GetRoleByIdResponse> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var role = await roles.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Role), request.Id);

        return role.Adapt<GetRoleByIdResponse>();
    }
}
