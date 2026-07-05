namespace Sentry.OS.IdentityServer.Application.Features.Authentication.TwoFactor.VerifyTwoFactorCode;

public record VerifyTwoFactorCodeResponse(bool Succeeded, string? ErrorCode, string? ErrorDescription, string? AuthorizationCode);
