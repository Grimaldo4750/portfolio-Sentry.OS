namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>A signed JWT and the UTC instant it expires at.</summary>
public record JwtIssuanceResult(string Token, DateTimeOffset ExpiresAtUtc);
