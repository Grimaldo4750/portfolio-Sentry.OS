using Sentry.OS.Domain.Applications;
using Sentry.OS.Domain.Common;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Tokens;

namespace Sentry.OS.Domain.Clients;

/// <summary>An OAuth 2.0 / OIDC client belonging to an application.</summary>
public class Client : AuditableEntity, IOrganizationScoped
{
    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <summary>Owning application.</summary>
    public Guid ApplicationId { get; set; }

    /// <summary>Public, globally-unique client identifier.</summary>
    public string ClientId { get; set; } = null!;

    /// <summary>Display name.</summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>Hashed client secret (null for public/PKCE-only clients). Never plaintext.</summary>
    public string? ClientSecretHash { get; set; }

    /// <summary>Whether PKCE is required.</summary>
    public bool RequirePkce { get; set; } = true;

    /// <summary>Whether a client secret is required.</summary>
    public bool RequireClientSecret { get; set; }

    /// <summary>Access-token lifetime in seconds.</summary>
    public int AccessTokenLifetimeSeconds { get; set; } = 3600;

    /// <summary>Identity-token lifetime in seconds.</summary>
    public int IdentityTokenLifetimeSeconds { get; set; } = 300;

    /// <summary>Refresh-token lifetime in seconds.</summary>
    public int RefreshTokenLifetimeSeconds { get; set; } = 1209600;

    /// <summary>Whether refresh-token rotation is enabled.</summary>
    public bool RefreshTokenRotationEnabled { get; set; } = true;

    /// <summary>Whether the client is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Owning application.</summary>
    public Application Application { get; set; } = null!;

    /// <summary>Allowed redirect URIs.</summary>
    public ICollection<ClientRedirectUri> RedirectUris { get; set; } = new List<ClientRedirectUri>();

    /// <summary>Allowed CORS origins.</summary>
    public ICollection<ClientCorsOrigin> CorsOrigins { get; set; } = new List<ClientCorsOrigin>();

    /// <summary>Allowed grant types.</summary>
    public ICollection<ClientGrantType> GrantTypes { get; set; } = new List<ClientGrantType>();

    /// <summary>Allowed scopes (join to <see cref="Resources.Scope"/>).</summary>
    public ICollection<ClientAllowedScope> AllowedScopes { get; set; } = new List<ClientAllowedScope>();

    /// <summary>Refresh tokens issued for this client.</summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
