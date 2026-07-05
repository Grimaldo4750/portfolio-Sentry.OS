namespace Sentry.OS.Domain.Users;

/// <summary>The purpose of a one-time <see cref="UserToken"/>.</summary>
public enum UserTokenPurpose
{
    /// <summary>Email address verification.</summary>
    EmailVerification = 0,

    /// <summary>Password reset.</summary>
    PasswordReset = 1,

    /// <summary>Email-based two-factor authentication code.</summary>
    TwoFactor = 2
}

/// <summary>
/// A short-lived, single-use secret issued to a user (email verification, password reset, or 2FA).
/// The value is stored hashed; the plaintext is only ever sent to the user.
/// </summary>
public class UserToken
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Owning user.</summary>
    public Guid UserId { get; set; }

    /// <summary>What the token authorizes.</summary>
    public UserTokenPurpose Purpose { get; set; }

    /// <summary>Hash of the token value (never plaintext).</summary>
    public string TokenHash { get; set; } = null!;

    /// <summary>UTC expiry.</summary>
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>UTC time the token was consumed, if used.</summary>
    public DateTime? ConsumedAtUtc { get; set; }

    /// <summary>UTC creation time.</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Navigation to the owning user.</summary>
    public User User { get; set; } = null!;
}
