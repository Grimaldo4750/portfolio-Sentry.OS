namespace Sentry.OS.IdentityServer.Application.Features.Authentication.SignIn;

/// <summary>
/// Result of a sign-in attempt. When <see cref="TwoFactorRequired"/> is <see langword="true"/>, no
/// authorization code has been issued yet — the caller must complete
/// <c>VerifyTwoFactorCode</c> using <see cref="PendingUserId"/> before one is granted.
/// </summary>
public record SignInResponse(
    bool Succeeded,
    string? ErrorCode,
    string? ErrorDescription,
    bool TwoFactorRequired,
    Guid? PendingUserId,
    string? AuthorizationCode);
