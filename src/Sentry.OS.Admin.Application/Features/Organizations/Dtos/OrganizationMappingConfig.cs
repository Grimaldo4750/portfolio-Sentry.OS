using Mapster;
using Sentry.OS.Domain.Organizations;

namespace Sentry.OS.Admin.Application.Features.Organizations.Dtos;

public class OrganizationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Organization, OrganizationDto>();
    }
}
