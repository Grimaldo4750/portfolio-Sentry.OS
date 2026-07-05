using Sentry.OS.Admin.Application.Features.Scopes.Dtos;

namespace Sentry.OS.Admin.Application.Features.ApiResources.Dtos;

public class ApiResourceDto
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public bool IsActive { get; set; }

    public List<ScopeDto> Scopes { get; set; } = [];
}

public class ApiResourceCreateRequest
{
    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;
}

public class ApiResourceUpdateRequest
{
    public string DisplayName { get; set; } = null!;
}
