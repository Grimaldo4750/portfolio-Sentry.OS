namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>Everything a one-time authorization code is bound to (RFC 6749 + PKCE, RFC 7636).</summary>
public record AuthorizationCodeData(
    Guid ClientId,
    Guid UserId,
    Guid OrganizationId,
    IReadOnlyList<string> RequestedScopes,
    string RedirectUri,
    string CodeChallenge,
    string CodeChallengeMethod,
    string? Nonce);
