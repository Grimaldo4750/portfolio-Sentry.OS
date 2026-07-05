using MediatR;
using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.Application.Features.Users.ListUsers;

public class ListUsersQuery : PagingRequest, IRequest<ListUsersResponse>
{
    public Guid OrganizationId { get; set; }
}
