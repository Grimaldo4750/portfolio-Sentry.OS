namespace Sentry.OS.IdentityServer.Application.Features.Tokens.ExchangeAuthorizationCode;

public record ExchangeAuthorizationCodeResponse(
    bool Succeeded,
    string? ErrorCode,
    string? ErrorDescription,
    string? AccessToken,
    string? IdToken,
    string? RefreshToken,
    int ExpiresInSeconds,
    string? Scope);
