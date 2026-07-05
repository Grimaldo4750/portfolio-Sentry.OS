using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.ReplaceClientAllowedScopes;

public class ReplaceClientAllowedScopesHandler(IClientRepository clients, ICurrentActor currentActor)
    : IRequestHandler<ReplaceClientAllowedScopesCommand, ReplaceClientAllowedScopesResponse>
{
    public async Task<ReplaceClientAllowedScopesResponse> Handle(
        ReplaceClientAllowedScopesCommand request, CancellationToken cancellationToken)
    {
        var client = await clients.GetByIdAsync(request.ApplicationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Client), request.Id);

        currentActor.EnsureOrganizationAccess(client.OrganizationId);

        var distinctScopeIds = request.ScopeIds.Distinct().ToList();
        if (distinctScopeIds.Count != 0)
        {
            var validCount = await clients.CountScopesBelongingToApplicationAsync(
                request.ApplicationId, distinctScopeIds, cancellationToken);

            if (validCount != distinctScopeIds.Count)
            {
                throw new ConflictException("Every scope must belong to an API resource owned by the client's application.");
            }
        }

        client.AllowedScopes.Clear();
        foreach (var scopeId in distinctScopeIds)
        {
            client.AllowedScopes.Add(new ClientAllowedScope { ClientId = client.Id, ScopeId = scopeId });
        }

        await clients.SaveChangesAsync(cancellationToken);

        var updated = await clients.GetByIdAsync(request.ApplicationId, client.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Client), client.Id);

        return updated.Adapt<ReplaceClientAllowedScopesResponse>();
    }
}
