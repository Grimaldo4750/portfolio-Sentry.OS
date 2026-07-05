using MediatR;
using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.Application.Features.Clients.ListClients;

public class ListClientsQuery : PagingRequest, IRequest<ListClientsResponse>
{
    public Guid ApplicationId { get; set; }
}
