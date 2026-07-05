using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Admin.Application.Features.Scopes.Dtos;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.ListScopes;

public class ListScopesHandler(IScopeRepository scopes, IApiResourceRepository apiResources, ICurrentActor currentActor)
    : IRequestHandler<ListScopesQuery, ListScopesResponse>
{
    public async Task<ListScopesResponse> Handle(ListScopesQuery request, CancellationToken cancellationToken)
    {
        var apiResource = await apiResources.GetByIdAsync(request.ApiResourceId, cancellationToken)
            ?? throw new NotFoundException(nameof(ApiResource), request.ApiResourceId);

        currentActor.EnsureOrganizationAccess(apiResource.OrganizationId);

        var (items, totalCount) = await scopes.ListAsync(request.ApiResourceId, request.Page, request.PageSize, cancellationToken);

        return new ListScopesResponse
        {
            Items = items.Adapt<List<ScopeDto>>(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
