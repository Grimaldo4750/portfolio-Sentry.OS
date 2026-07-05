using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.RefreshTokens;

/// <summary>Renews access/identity tokens using a refresh token, rotating it with reuse detection (FR-009).</summary>
public record RefreshTokensCommand(string RefreshToken, string ClientId) : IRequest<RefreshTokensResponse>;
