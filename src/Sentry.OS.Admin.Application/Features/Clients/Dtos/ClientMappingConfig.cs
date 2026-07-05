using Mapster;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.Dtos;

public class ClientMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Client, ClientDto>()
            .Map(dest => dest.AllowedScopeNames, src => src.AllowedScopes.Select(a => a.Scope.Name).ToList());
    }
}
