using Mapster;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.Dtos;

public class ScopeMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Scope, ScopeDto>();
    }
}
