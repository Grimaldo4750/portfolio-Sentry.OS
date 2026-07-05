using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.TwoFactor.VerifyTwoFactorCode;

/// <summary>Completes sign-in for a two-factor-enabled user by verifying the emailed one-time code (FR-032).</summary>
public record VerifyTwoFactorCodeCommand(
    Guid UserId,
    string Code,
    string ClientId,
    string RedirectUri,
    IReadOnlyList<string> RequestedScopes,
    string CodeChallenge,
    string CodeChallengeMethod,
    string? State,
    string? Nonce) : IRequest<VerifyTwoFactorCodeResponse>;
