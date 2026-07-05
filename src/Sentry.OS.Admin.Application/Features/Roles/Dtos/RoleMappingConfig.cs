using Mapster;
using Sentry.OS.Domain.Authorization;

namespace Sentry.OS.Admin.Application.Features.Roles.Dtos;

public class RoleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Role, RoleDto>()
            .Map(dest => dest.ScopeNames, src => src.RoleScopes.Select(rs => rs.Scope.Name).ToList());
    }
}
