using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Authorization.Authorize;

/// <summary>Validates an incoming <c>GET /connect/authorize</c> request before the login step is shown.</summary>
public record AuthorizeQuery(
    string ClientId,
    string RedirectUri,
    string ResponseType,
    IReadOnlyList<string> RequestedScopes,
    string CodeChallenge,
    string CodeChallengeMethod,
    string? State,
    string? Nonce) : IRequest<AuthorizeResponse>;
