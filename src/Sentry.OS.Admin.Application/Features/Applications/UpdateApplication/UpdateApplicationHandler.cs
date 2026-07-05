using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Applications.UpdateApplication;

public class UpdateApplicationHandler(IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<UpdateApplicationCommand, UpdateApplicationResponse>
{
    public async Task<UpdateApplicationResponse> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var application = await applications.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainApplication), request.Id);

        application.Name = request.Name;
        application.Description = request.Description;

        await applications.SaveChangesAsync(cancellationToken);

        return application.Adapt<UpdateApplicationResponse>();
    }
}
