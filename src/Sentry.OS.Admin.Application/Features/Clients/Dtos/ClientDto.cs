namespace Sentry.OS.Admin.Application.Features.Clients.Dtos;

public class ClientDto
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }

    public string ClientId { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public bool RequirePkce { get; set; }

    public bool RequireClientSecret { get; set; }

    public int AccessTokenLifetimeSeconds { get; set; }

    public int IdentityTokenLifetimeSeconds { get; set; }

    public int RefreshTokenLifetimeSeconds { get; set; }

    public bool RefreshTokenRotationEnabled { get; set; }

    public bool IsActive { get; set; }

    public List<string> AllowedScopeNames { get; set; } = [];
}

public class ClientCreateRequest
{
    public string DisplayName { get; set; } = null!;

    public bool RequirePkce { get; set; } = true;

    public bool RequireClientSecret { get; set; }

    public int AccessTokenLifetimeSeconds { get; set; } = 3600;

    public int IdentityTokenLifetimeSeconds { get; set; } = 300;

    public int RefreshTokenLifetimeSeconds { get; set; } = 1209600;

    public bool RefreshTokenRotationEnabled { get; set; } = true;
}

public class ClientUpdateRequest
{
    public string DisplayName { get; set; } = null!;

    public bool RequirePkce { get; set; }

    public bool RequireClientSecret { get; set; }

    public int AccessTokenLifetimeSeconds { get; set; }

    public int IdentityTokenLifetimeSeconds { get; set; }

    public int RefreshTokenLifetimeSeconds { get; set; }

    public bool RefreshTokenRotationEnabled { get; set; }
}

public class ReplaceClientAllowedScopesRequest
{
    public List<Guid> ScopeIds { get; set; } = [];
}
