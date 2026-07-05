using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.Clients.Dtos;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Clients.ListClients;

public class ListClientsHandler(IClientRepository clients, IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<ListClientsQuery, ListClientsResponse>
{
    public async Task<ListClientsResponse> Handle(ListClientsQuery request, CancellationToken cancellationToken)
    {
        var application = await applications.GetByIdIgnoringOrganizationAsync(request.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainApplication), request.ApplicationId);

        currentActor.EnsureOrganizationAccess(application.OrganizationId);

        var (items, totalCount) = await clients.ListAsync(request.ApplicationId, request.Page, request.PageSize, cancellationToken);

        return new ListClientsResponse
        {
            Items = items.Adapt<List<ClientDto>>(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
