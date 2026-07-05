using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.ApiResources.DeleteApiResource;

public class DeleteApiResourceHandler(IApiResourceRepository apiResources, ICurrentActor currentActor)
    : IRequestHandler<DeleteApiResourceCommand, Unit>
{
    public async Task<Unit> Handle(DeleteApiResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await apiResources.GetByIdWithScopesAsync(request.ApplicationId, request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ApiResource), request.Id);

        currentActor.EnsureOrganizationAccess(resource.OrganizationId);

        if (resource.Scopes.Count != 0)
        {
            throw new ConflictException("This API resource has scopes and cannot be deleted.");
        }

        apiResources.Remove(resource);
        await apiResources.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
