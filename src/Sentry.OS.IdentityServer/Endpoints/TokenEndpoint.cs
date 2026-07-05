using MediatR;
using Sentry.OS.IdentityServer.Application.Features.Tokens.ExchangeAuthorizationCode;
using Sentry.OS.IdentityServer.Application.Features.Tokens.RefreshTokens;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps <c>POST /connect/token</c>. Handles <c>grant_type=authorization_code</c> and <c>grant_type=refresh_token</c>.</summary>
public static class TokenEndpoint
{
    public static IEndpointRouteBuilder MapTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/connect/token", async (HttpRequest httpRequest, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var form = await httpRequest.ReadFormAsync(cancellationToken);
            var grantType = form["grant_type"].ToString();

            return grantType switch
            {
                "authorization_code" => await HandleAuthorizationCodeAsync(form, mediator, cancellationToken),
                "refresh_token" => await HandleRefreshTokenAsync(form, mediator, cancellationToken),
                _ => Results.BadRequest(new { error = "unsupported_grant_type", error_description = "The requested grant type is not supported." })
            };
        })
        .WithName("Token")
        .WithTags("OAuth2 / OIDC Protocol")
        .WithSummary("Exchanges an authorization code or refresh token for access, identity, and refresh tokens.")
        .RequireRateLimiting("auth");

        return app;
    }

    private static async Task<IResult> HandleAuthorizationCodeAsync(IFormCollection form, IMediator mediator, CancellationToken cancellationToken)
    {
        var code = form["code"].ToString();
        var redirectUri = form["redirect_uri"].ToString();
        var clientId = form["client_id"].ToString();
        var codeVerifier = form["code_verifier"].ToString();

        var result = await mediator.Send(new ExchangeAuthorizationCodeCommand(code, redirectUri, clientId, codeVerifier), cancellationToken);

        if (!result.Succeeded)
        {
            return Results.BadRequest(new { error = result.ErrorCode, error_description = result.ErrorDescription });
        }

        return Results.Ok(new
        {
            access_token = result.AccessToken,
            id_token = result.IdToken,
            refresh_token = result.RefreshToken,
            token_type = "Bearer",
            expires_in = result.ExpiresInSeconds,
            scope = result.Scope
        });
    }

    private static async Task<IResult> HandleRefreshTokenAsync(IFormCollection form, IMediator mediator, CancellationToken cancellationToken)
    {
        var refreshToken = form["refresh_token"].ToString();
        var clientId = form["client_id"].ToString();

        var result = await mediator.Send(new RefreshTokensCommand(refreshToken, clientId), cancellationToken);

        if (!result.Succeeded)
        {
            return Results.BadRequest(new { error = result.ErrorCode, error_description = result.ErrorDescription });
        }

        return Results.Ok(new
        {
            access_token = result.AccessToken,
            id_token = result.IdToken,
            refresh_token = result.RefreshToken,
            token_type = "Bearer",
            expires_in = result.ExpiresInSeconds,
            scope = result.Scope
        });
    }
}
