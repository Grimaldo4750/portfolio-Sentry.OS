using Mapster;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.ApiResources.Dtos;

public class ApiResourceMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ApiResource, ApiResourceDto>();
    }
}
