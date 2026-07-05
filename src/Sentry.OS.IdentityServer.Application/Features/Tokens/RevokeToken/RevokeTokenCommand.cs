using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.RevokeToken;

/// <summary>Revokes a refresh token and its forward lineage so it can no longer renew tokens (FR-010, RFC 7009).</summary>
public record RevokeTokenCommand(string Token) : IRequest<RevokeTokenResponse>;
