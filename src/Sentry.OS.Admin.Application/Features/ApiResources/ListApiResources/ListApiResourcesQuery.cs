using MediatR;
using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.Application.Features.ApiResources.ListApiResources;

public class ListApiResourcesQuery : PagingRequest, IRequest<ListApiResourcesResponse>
{
    public Guid ApplicationId { get; set; }
}
