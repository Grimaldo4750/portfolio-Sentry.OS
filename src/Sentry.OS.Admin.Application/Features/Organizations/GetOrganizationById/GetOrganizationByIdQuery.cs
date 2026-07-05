using MediatR;

namespace Sentry.OS.Admin.Application.Features.Organizations.GetOrganizationById;

public record GetOrganizationByIdQuery(Guid Id) : IRequest<GetOrganizationByIdResponse>;
