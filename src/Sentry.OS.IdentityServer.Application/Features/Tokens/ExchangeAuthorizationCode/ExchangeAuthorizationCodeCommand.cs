using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.ExchangeAuthorizationCode;

/// <summary>Exchanges a one-time authorization code for access, identity, and refresh tokens (FR-004, FR-008).</summary>
public record ExchangeAuthorizationCodeCommand(
    string Code,
    string RedirectUri,
    string ClientId,
    string CodeVerifier) : IRequest<ExchangeAuthorizationCodeResponse>;
