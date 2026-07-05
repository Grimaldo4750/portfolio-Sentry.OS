using MediatR;
using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.Application.Features.Scopes.ListScopes;

public class ListScopesQuery : PagingRequest, IRequest<ListScopesResponse>
{
    public Guid ApiResourceId { get; set; }
}
