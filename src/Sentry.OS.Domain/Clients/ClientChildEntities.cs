using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Domain.Clients;

/// <summary>An allowed redirect URI for a client.</summary>
public class ClientRedirectUri
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Owning client.</summary>
    public Guid ClientId { get; set; }

    /// <summary>Redirect URI.</summary>
    public string Uri { get; set; } = null!;

    /// <summary>Navigation to the owning client.</summary>
    public Client Client { get; set; } = null!;
}

/// <summary>An allowed CORS origin for a client.</summary>
public class ClientCorsOrigin
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Owning client.</summary>
    public Guid ClientId { get; set; }

    /// <summary>Allowed origin.</summary>
    public string Origin { get; set; } = null!;

    /// <summary>Navigation to the owning client.</summary>
    public Client Client { get; set; } = null!;
}

/// <summary>An allowed grant type for a client (e.g. <c>authorization_code</c>).</summary>
public class ClientGrantType
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Owning client.</summary>
    public Guid ClientId { get; set; }

    /// <summary>Grant type value.</summary>
    public string GrantType { get; set; } = null!;

    /// <summary>Navigation to the owning client.</summary>
    public Client Client { get; set; } = null!;
}

/// <summary>Join entity linking a client to a scope it is allowed to request.</summary>
public class ClientAllowedScope
{
    /// <summary>Owning client.</summary>
    public Guid ClientId { get; set; }

    /// <summary>Allowed scope.</summary>
    public Guid ScopeId { get; set; }

    /// <summary>Navigation to the client.</summary>
    public Client Client { get; set; } = null!;

    /// <summary>Navigation to the scope.</summary>
    public Scope Scope { get; set; } = null!;
}
