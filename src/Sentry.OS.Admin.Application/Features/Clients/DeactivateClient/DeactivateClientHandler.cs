using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.DeactivateClient;

public class DeactivateClientHandler(IClientRepository clients, ICurrentActor currentActor)
    : IRequestHandler<DeactivateClientCommand, DeactivateClientResponse>
{
    public async Task<DeactivateClientResponse> Handle(DeactivateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await clients.GetByIdAsync(request.ApplicationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Client), request.Id);

        currentActor.EnsureOrganizationAccess(client.OrganizationId);

        client.IsActive = false;

        await clients.SaveChangesAsync(cancellationToken);

        return client.Adapt<DeactivateClientResponse>();
    }
}
