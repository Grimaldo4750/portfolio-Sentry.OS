using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Resources;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.ApiResources.CreateApiResource;

public class CreateApiResourceHandler(IApiResourceRepository apiResources, IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<CreateApiResourceCommand, CreateApiResourceResponse>
{
    public async Task<CreateApiResourceResponse> Handle(CreateApiResourceCommand request, CancellationToken cancellationToken)
    {
        var application = await applications.GetByIdIgnoringOrganizationAsync(request.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainApplication), request.ApplicationId);

        currentActor.EnsureOrganizationAccess(application.OrganizationId);

        if (await apiResources.NameExistsAsync(request.ApplicationId, request.Name, cancellationToken))
        {
            throw new ConflictException("An API resource with this name already exists in the application.");
        }

        var resource = new ApiResource
        {
            OrganizationId = application.OrganizationId,
            ApplicationId = request.ApplicationId,
            Name = request.Name,
            DisplayName = request.DisplayName,
            IsActive = true
        };

        apiResources.Add(resource);
        await apiResources.SaveChangesAsync(cancellationToken);

        return resource.Adapt<CreateApiResourceResponse>();
    }
}
