using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.UpdateScope;

public class UpdateScopeHandler(IScopeRepository scopes, ICurrentActor currentActor)
    : IRequestHandler<UpdateScopeCommand, UpdateScopeResponse>
{
    public async Task<UpdateScopeResponse> Handle(UpdateScopeCommand request, CancellationToken cancellationToken)
    {
        var scope = await scopes.GetByIdAsync(request.ApiResourceId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Scope), request.Id);

        currentActor.EnsureOrganizationAccess(scope.OrganizationId);

        scope.DisplayName = request.DisplayName;
        scope.Description = request.Description;

        await scopes.SaveChangesAsync(cancellationToken);

        return scope.Adapt<UpdateScopeResponse>();
    }
}
