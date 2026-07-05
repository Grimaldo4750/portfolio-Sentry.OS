using MediatR;

namespace Sentry.OS.Admin.Application.Features.Scopes.GetScopeById;

public record GetScopeByIdQuery(Guid ApiResourceId, Guid Id) : IRequest<GetScopeByIdResponse>;
