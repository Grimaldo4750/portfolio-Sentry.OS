using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.DeactivateUser;

public class DeactivateUserHandler(IUserRepository users, ICurrentActor currentActor)
    : IRequestHandler<DeactivateUserCommand, DeactivateUserResponse>
{
    public async Task<DeactivateUserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var user = await users.GetInOrganizationAsync(request.OrganizationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

        user.IsDisabled = true;

        await users.SaveChangesAsync(cancellationToken);

        return user.Adapt<DeactivateUserResponse>();
    }
}
