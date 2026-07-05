using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.GetScopeById;

public class GetScopeByIdHandler(IScopeRepository scopes, ICurrentActor currentActor)
    : IRequestHandler<GetScopeByIdQuery, GetScopeByIdResponse>
{
    public async Task<GetScopeByIdResponse> Handle(GetScopeByIdQuery request, CancellationToken cancellationToken)
    {
        var scope = await scopes.GetByIdAsync(request.ApiResourceId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Scope), request.Id);

        currentActor.EnsureOrganizationAccess(scope.OrganizationId);

        return scope.Adapt<GetScopeByIdResponse>();
    }
}
