using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Common;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Tokens;

namespace Sentry.OS.Domain.Users;

/// <summary>
/// A platform-global identity with credentials and security/lifecycle state. Users gain
/// organization access via <see cref="OrganizationMembership"/>; they are not themselves
/// organization-scoped.
/// </summary>
public class User : AuditableEntity, ISoftDelete
{
    /// <summary>Login email (unique platform-wide).</summary>
    public string Email { get; set; } = null!;

    /// <summary>Upper-invariant email used for case-insensitive uniqueness/lookup.</summary>
    public string NormalizedEmail { get; set; } = null!;

    /// <summary>Whether the email has been verified.</summary>
    public bool EmailVerified { get; set; }

    /// <summary>Unique user name.</summary>
    public string UserName { get; set; } = null!;

    /// <summary>Self-describing PBKDF2 password hash. Never stores plaintext.</summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>Rotates on credential/security changes to invalidate outstanding sessions.</summary>
    public string SecurityStamp { get; set; } = null!;

    /// <summary>Given name.</summary>
    public string? FirstName { get; set; }

    /// <summary>Family name.</summary>
    public string? LastName { get; set; }

    /// <summary>External/CDN reference to the profile picture, if any.</summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>Whether two-factor authentication is enabled.</summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>Two-factor method (e.g. <c>Email</c>).</summary>
    public string? TwoFactorMethod { get; set; }

    /// <summary>Phone number, if provided.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Whether the phone number has been verified.</summary>
    public bool PhoneNumberVerified { get; set; }

    /// <summary>Whether the account has been administratively disabled.</summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Whether this user holds platform-wide administrative authority across all organizations,
    /// independent of any single <see cref="Organizations.OrganizationMembership"/>.
    /// </summary>
    public bool IsGlobalAdministrator { get; set; }

    /// <summary>Whether lockout on repeated failures is enabled.</summary>
    public bool LockoutEnabled { get; set; } = true;

    /// <summary>UTC time until which the account is locked out, if any.</summary>
    public DateTime? LockoutEndUtc { get; set; }

    /// <summary>Consecutive failed access attempts.</summary>
    public int AccessFailedCount { get; set; }

    /// <summary>UTC timestamp of the last successful login.</summary>
    public DateTime? LastLoginAtUtc { get; set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTime? DeletedAtUtc { get; set; }

    /// <summary>Organization memberships for this user.</summary>
    public ICollection<OrganizationMembership> Memberships { get; set; } = new List<OrganizationMembership>();

    /// <summary>Role assignments for this user.</summary>
    public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();

    /// <summary>Custom claims attached to this user.</summary>
    public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

    /// <summary>One-time lifecycle tokens (email verify / reset / 2FA).</summary>
    public ICollection<UserToken> Tokens { get; set; } = new List<UserToken>();

    /// <summary>Refresh tokens issued to this user.</summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>Optional inline profile picture (1:1).</summary>
    public UserProfilePicture? ProfilePicture { get; set; }
}
