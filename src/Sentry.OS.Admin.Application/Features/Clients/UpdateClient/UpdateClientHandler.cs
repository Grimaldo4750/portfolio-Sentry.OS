using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.UpdateClient;

public class UpdateClientHandler(IClientRepository clients, ICurrentActor currentActor)
    : IRequestHandler<UpdateClientCommand, UpdateClientResponse>
{
    public async Task<UpdateClientResponse> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await clients.GetByIdAsync(request.ApplicationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Client), request.Id);

        currentActor.EnsureOrganizationAccess(client.OrganizationId);

        client.DisplayName = request.DisplayName;
        client.RequirePkce = request.RequirePkce;
        client.RequireClientSecret = request.RequireClientSecret;
        client.AccessTokenLifetimeSeconds = request.AccessTokenLifetimeSeconds;
        client.IdentityTokenLifetimeSeconds = request.IdentityTokenLifetimeSeconds;
        client.RefreshTokenLifetimeSeconds = request.RefreshTokenLifetimeSeconds;
        client.RefreshTokenRotationEnabled = request.RefreshTokenRotationEnabled;

        await clients.SaveChangesAsync(cancellationToken);

        return client.Adapt<UpdateClientResponse>();
    }
}
