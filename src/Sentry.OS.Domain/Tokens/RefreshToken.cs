using Sentry.OS.Domain.Clients;
using Sentry.OS.Domain.Common;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Domain.Tokens;

/// <summary>Reason a refresh token was revoked.</summary>
public enum RefreshTokenRevocationReason
{
    /// <summary>Superseded by a rotated successor token.</summary>
    Rotated = 0,

    /// <summary>Revoked because the user logged out.</summary>
    Logout = 1,

    /// <summary>Explicitly revoked (e.g. by an administrator).</summary>
    Revoked = 2,

    /// <summary>Revoked because a consumed token was replayed (reuse detection).</summary>
    ReuseDetected = 3
}

/// <summary>
/// An issued refresh token. Append-only: rotation creates a successor and links it via
/// <see cref="ReplacedByTokenId"/>; presenting a token whose <see cref="ConsumedAtUtc"/> is set
/// indicates reuse.
/// </summary>
public class RefreshToken : IOrganizationScoped
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <summary>Owning user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Issuing client.</summary>
    public Guid ClientId { get; set; }

    /// <summary>Hash of the token value (unique; never plaintext).</summary>
    public string TokenHash { get; set; } = null!;

    /// <summary>UTC creation time.</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>UTC expiry.</summary>
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>UTC time the token was consumed (rotated), if any.</summary>
    public DateTime? ConsumedAtUtc { get; set; }

    /// <summary>UTC time the token was revoked, if any.</summary>
    public DateTime? RevokedAtUtc { get; set; }

    /// <summary>Reason for revocation, if revoked.</summary>
    public RefreshTokenRevocationReason? RevocationReason { get; set; }

    /// <summary>Successor token that replaced this one during rotation.</summary>
    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>Navigation to the owning user.</summary>
    public User User { get; set; } = null!;

    /// <summary>Navigation to the issuing client.</summary>
    public Client Client { get; set; } = null!;

    /// <summary>Navigation to the successor token, if rotated.</summary>
    public RefreshToken? ReplacedByToken { get; set; }
}
