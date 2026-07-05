namespace Sentry.OS.Admin.Application.Features.Scopes.Dtos;

public class ScopeDto
{
    public Guid Id { get; set; }

    public Guid ApiResourceId { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Description { get; set; }
}

public class ScopeCreateRequest
{
    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Description { get; set; }
}

public class ScopeUpdateRequest
{
    public string DisplayName { get; set; } = null!;

    public string? Description { get; set; }
}
