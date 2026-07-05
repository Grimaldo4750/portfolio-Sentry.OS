using MediatR;

namespace Sentry.OS.Admin.Application.Features.Users.ListUserRoleAssignments;

public record ListUserRoleAssignmentsQuery(Guid OrganizationId, Guid UserId) : IRequest<ListUserRoleAssignmentsResponse>;
