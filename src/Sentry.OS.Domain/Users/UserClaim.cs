using Sentry.OS.Domain.Common;

namespace Sentry.OS.Domain.Users;

/// <summary>A custom, typed claim attached to a platform-global user.</summary>
public class UserClaim : AuditableEntity
{
    /// <summary>Owning user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Claim type (e.g. <c>department</c>).</summary>
    public string ClaimType { get; set; } = null!;

    /// <summary>Claim value.</summary>
    public string ClaimValue { get; set; } = null!;

    /// <summary>Navigation to the owning user.</summary>
    public User User { get; set; } = null!;
}
