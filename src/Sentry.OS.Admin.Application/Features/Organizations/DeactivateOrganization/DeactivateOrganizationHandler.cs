using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Features.Organizations.DeactivateOrganization;

public class DeactivateOrganizationHandler(IOrganizationRepository organizations)
    : IRequestHandler<DeactivateOrganizationCommand, DeactivateOrganizationResponse>
{
    public async Task<DeactivateOrganizationResponse> Handle(DeactivateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizations.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Organization), request.Id);

        organization.IsActive = false;

        await organizations.SaveChangesAsync(cancellationToken);

        return organization.Adapt<DeactivateOrganizationResponse>();
    }
}
