namespace Sentry.OS.IdentityServer.Application.Features.Tokens.RefreshTokens;

public record RefreshTokensResponse(
    bool Succeeded,
    string? ErrorCode,
    string? ErrorDescription,
    string? AccessToken,
    string? IdToken,
    string? RefreshToken,
    int ExpiresInSeconds,
    string? Scope);
