using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Features.Organizations.GetOrganizationById;

public class GetOrganizationByIdHandler(IOrganizationRepository organizations)
    : IRequestHandler<GetOrganizationByIdQuery, GetOrganizationByIdResponse>
{
    public async Task<GetOrganizationByIdResponse> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var organization = await organizations.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Organization), request.Id);

        return organization.Adapt<GetOrganizationByIdResponse>();
    }
}
