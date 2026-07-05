using MediatR;

namespace Sentry.OS.Admin.Application.Features.ApiResources.GetApiResourceById;

public record GetApiResourceByIdQuery(Guid ApplicationId, Guid Id) : IRequest<GetApiResourceByIdResponse>;
