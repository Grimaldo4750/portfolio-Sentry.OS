using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.UpdateUser;

public class UpdateUserHandler(IUserRepository users, ICurrentActor currentActor)
    : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var user = await users.GetInOrganizationAsync(request.OrganizationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.ProfilePictureUrl = request.ProfilePictureUrl;

        await users.SaveChangesAsync(cancellationToken);

        return user.Adapt<UpdateUserResponse>();
    }
}
