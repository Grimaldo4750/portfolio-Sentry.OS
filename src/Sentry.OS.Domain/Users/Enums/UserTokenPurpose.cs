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
