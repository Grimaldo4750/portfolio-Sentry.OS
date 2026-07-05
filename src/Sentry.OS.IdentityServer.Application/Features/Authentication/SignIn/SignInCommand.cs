using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.SignIn;

/// <summary>Credential-entry step of the interactive authorization flow (posted from the login page).</summary>
public record SignInCommand(
    string Email,
    string Password,
    string ClientId,
    string RedirectUri,
    IReadOnlyList<string> RequestedScopes,
    string CodeChallenge,
    string CodeChallengeMethod,
    string? State,
    string? Nonce) : IRequest<SignInResponse>;
