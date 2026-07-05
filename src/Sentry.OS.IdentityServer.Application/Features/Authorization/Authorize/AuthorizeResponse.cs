namespace Sentry.OS.IdentityServer.Application.Features.Authorization.Authorize;

/// <summary>
/// Result of validating an authorize request. <see cref="CanRedirectToClient"/> is <see langword="false"/>
/// only when the client id or redirect location itself is the problem (an unregistered redirect target
/// cannot be trusted); otherwise the caller may redirect back to the client with an error.
/// </summary>
public record AuthorizeResponse(bool IsValid, string? ErrorCode, string? ErrorDescription, bool CanRedirectToClient);
