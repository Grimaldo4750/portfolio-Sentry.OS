using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Domain.Authorization;

/// <summary>Join entity assigning a scope to a role.</summary>
public class RoleScope
{
    /// <summary>Owning role.</summary>
    public Guid RoleId { get; set; }

    /// <summary>Granted scope.</summary>
    public Guid ScopeId { get; set; }

    /// <summary>Navigation to the role.</summary>
    public Role Role { get; set; } = null!;

    /// <summary>Navigation to the scope.</summary>
    public Scope Scope { get; set; } = null!;
}
