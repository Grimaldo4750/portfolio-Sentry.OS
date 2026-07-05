using MediatR;

namespace Sentry.OS.Admin.Application.Features.Clients.GetClientById;

public record GetClientByIdQuery(Guid ApplicationId, Guid Id) : IRequest<GetClientByIdResponse>;
