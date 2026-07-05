namespace Sentry.OS.Admin.Application.Features.Users.Dtos;

public class UserDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public bool IsDisabled { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LastLoginAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}

public class UserCreateRequest
{
    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}

public class UserUpdateRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ProfilePictureUrl { get; set; }
}

public class RoleAssignmentRequest
{
    public Guid RoleId { get; set; }
}

public class RoleAssignmentDto
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public DateTime AssignedAtUtc { get; set; }
}
