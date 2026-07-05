using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Applications.GetApplicationById;

public class GetApplicationByIdHandler(IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<GetApplicationByIdQuery, GetApplicationByIdResponse>
{
    public async Task<GetApplicationByIdResponse> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var application = await applications.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainApplication), request.Id);

        return application.Adapt<GetApplicationByIdResponse>();
    }
}
