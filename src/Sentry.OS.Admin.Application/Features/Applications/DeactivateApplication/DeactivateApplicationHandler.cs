using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Applications.DeactivateApplication;

public class DeactivateApplicationHandler(IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<DeactivateApplicationCommand, DeactivateApplicationResponse>
{
    public async Task<DeactivateApplicationResponse> Handle(DeactivateApplicationCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var application = await applications.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainApplication), request.Id);

        application.IsActive = false;

        await applications.SaveChangesAsync(cancellationToken);

        return application.Adapt<DeactivateApplicationResponse>();
    }
}
