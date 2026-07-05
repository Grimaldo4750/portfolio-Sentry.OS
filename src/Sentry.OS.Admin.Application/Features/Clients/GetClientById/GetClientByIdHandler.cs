using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.GetClientById;

public class GetClientByIdHandler(IClientRepository clients, ICurrentActor currentActor)
    : IRequestHandler<GetClientByIdQuery, GetClientByIdResponse>
{
    public async Task<GetClientByIdResponse> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await clients.GetByIdAsync(request.ApplicationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Client), request.Id);

        currentActor.EnsureOrganizationAccess(client.OrganizationId);

        return client.Adapt<GetClientByIdResponse>();
    }
}
