using Mapster;
using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.Dtos;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>();
        config.NewConfig<RoleAssignment, RoleAssignmentDto>()
            .Map(dest => dest.RoleName, src => src.Role.Name);
    }
}
