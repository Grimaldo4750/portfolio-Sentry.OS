namespace Sentry.OS.Admin.Application.Features.Roles.Dtos;

public class RoleDto
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Level { get; set; }

    public List<string> ScopeNames { get; set; } = [];
}

public class RoleCreateRequest
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Level { get; set; }
}

public class RoleUpdateRequest
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Level { get; set; }
}

public class RoleScopeRequest
{
    public Guid ScopeId { get; set; }
}
