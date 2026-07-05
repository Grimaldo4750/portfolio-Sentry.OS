using MediatR;
using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.Application.Features.Applications.ListApplications;

public class ListApplicationsQuery : PagingRequest, IRequest<ListApplicationsResponse>
{
    public Guid OrganizationId { get; set; }
}
