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
