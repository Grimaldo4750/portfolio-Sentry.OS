using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.GetUserById;

public class GetUserByIdHandler(IUserRepository users, ICurrentActor currentActor)
    : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var user = await users.GetInOrganizationAsync(request.OrganizationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

        return user.Adapt<GetUserByIdResponse>();
    }
}
