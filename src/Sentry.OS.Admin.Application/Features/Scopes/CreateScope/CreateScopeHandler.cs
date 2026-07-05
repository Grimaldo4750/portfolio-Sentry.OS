using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.CreateScope;

public class CreateScopeHandler(IScopeRepository scopes, IApiResourceRepository apiResources, ICurrentActor currentActor)
    : IRequestHandler<CreateScopeCommand, CreateScopeResponse>
{
    public async Task<CreateScopeResponse> Handle(CreateScopeCommand request, CancellationToken cancellationToken)
    {
        var apiResource = await apiResources.GetByIdAsync(request.ApiResourceId, cancellationToken)
            ?? throw new NotFoundException(nameof(ApiResource), request.ApiResourceId);

        currentActor.EnsureOrganizationAccess(apiResource.OrganizationId);

        if (await scopes.NameExistsAsync(request.ApiResourceId, request.Name, cancellationToken))
        {
            throw new ConflictException("A scope with this name already exists in the API resource.");
        }

        var scope = new Scope
        {
            OrganizationId = apiResource.OrganizationId,
            ApiResourceId = request.ApiResourceId,
            Name = request.Name,
            DisplayName = request.DisplayName,
            Description = request.Description
        };

        scopes.Add(scope);
        await scopes.SaveChangesAsync(cancellationToken);

        return scope.Adapt<CreateScopeResponse>();
    }
}
