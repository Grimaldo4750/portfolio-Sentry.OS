using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Applications.CreateApplication;

public class CreateApplicationHandler(IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<CreateApplicationCommand, CreateApplicationResponse>
{
    public async Task<CreateApplicationResponse> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        if (await applications.SlugExistsAsync(request.OrganizationId, request.Slug, cancellationToken))
        {
            throw new ConflictException("An application with this slug already exists in the organization.");
        }

        var application = new DomainApplication
        {
            OrganizationId = request.OrganizationId,
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            IsActive = true
        };

        applications.Add(application);
        await applications.SaveChangesAsync(cancellationToken);

        return application.Adapt<CreateApplicationResponse>();
    }
}
