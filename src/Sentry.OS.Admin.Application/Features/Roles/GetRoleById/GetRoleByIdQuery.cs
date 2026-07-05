using MediatR;

namespace Sentry.OS.Admin.Application.Features.Roles.GetRoleById;

public record GetRoleByIdQuery(Guid OrganizationId, Guid Id) : IRequest<GetRoleByIdResponse>;
