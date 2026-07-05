using MediatR;

namespace Sentry.OS.Admin.Application.Features.Applications.GetApplicationById;

public record GetApplicationByIdQuery(Guid OrganizationId, Guid Id) : IRequest<GetApplicationByIdResponse>;
