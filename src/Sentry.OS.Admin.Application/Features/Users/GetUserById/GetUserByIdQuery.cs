using MediatR;

namespace Sentry.OS.Admin.Application.Features.Users.GetUserById;

public record GetUserByIdQuery(Guid OrganizationId, Guid Id) : IRequest<GetUserByIdResponse>;
