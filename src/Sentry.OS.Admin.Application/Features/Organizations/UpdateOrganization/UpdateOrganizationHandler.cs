using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Features.Organizations.UpdateOrganization;

public class UpdateOrganizationHandler(IOrganizationRepository organizations)
    : IRequestHandler<UpdateOrganizationCommand, UpdateOrganizationResponse>
{
    public async Task<UpdateOrganizationResponse> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizations.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Organization), request.Id);

        organization.Name = request.Name;
        organization.DisplayName = request.DisplayName;

        await organizations.SaveChangesAsync(cancellationToken);

        return organization.Adapt<UpdateOrganizationResponse>();
    }
}
