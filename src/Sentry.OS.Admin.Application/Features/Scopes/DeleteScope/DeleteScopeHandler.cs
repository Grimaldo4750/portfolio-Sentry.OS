using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.DeleteScope;

public class DeleteScopeHandler(IScopeRepository scopes, ICurrentActor currentActor)
    : IRequestHandler<DeleteScopeCommand, Unit>
{
    public async Task<Unit> Handle(DeleteScopeCommand request, CancellationToken cancellationToken)
    {
        var scope = await scopes.GetByIdWithUsageAsync(request.ApiResourceId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Scope), request.Id);

        currentActor.EnsureOrganizationAccess(scope.OrganizationId);

        if (scope.RoleScopes.Count != 0 || scope.ClientAllowedScopes.Count != 0)
        {
            throw new ConflictException("This scope is in use by roles or clients and cannot be deleted.");
        }

        scopes.Remove(scope);
        await scopes.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
