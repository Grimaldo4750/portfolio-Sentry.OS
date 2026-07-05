using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Features.Organizations.CreateOrganization;

public class CreateOrganizationHandler(IOrganizationRepository organizations)
    : IRequestHandler<CreateOrganizationCommand, CreateOrganizationResponse>
{
    public async Task<CreateOrganizationResponse> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (await organizations.SlugExistsAsync(request.Slug, cancellationToken))
        {
            throw new ConflictException("An organization with this slug already exists.");
        }

        var organization = new Organization
        {
            Name = request.Name,
            Slug = request.Slug,
            DisplayName = request.DisplayName,
            IsActive = true
        };

        organizations.Add(organization);
        await organizations.SaveChangesAsync(cancellationToken);

        return organization.Adapt<CreateOrganizationResponse>();
    }
}
