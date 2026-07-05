using MediatR;
using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.Application.Features.Roles.ListRoles;

public class ListRolesQuery : PagingRequest, IRequest<ListRolesResponse>
{
    public Guid OrganizationId { get; set; }
}
