using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Sentry.OS.IdentityServer.Application.Features.Authorization.Authorize;
using Sentry.OS.IdentityServer.Pages;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps <c>GET /connect/authorize</c> — the entry point of the Authorization Code + PKCE flow.</summary>
public static class AuthorizeEndpoint
{
    public static IEndpointRouteBuilder MapAuthorizeEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/connect/authorize", async (HttpRequest httpRequest, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = httpRequest.Query;
            var clientId = query["client_id"].ToString();
            var redirectUri = query["redirect_uri"].ToString();
            var responseType = query["response_type"].ToString();
            var scope = query["scope"].ToString();
            var codeChallenge = query["code_challenge"].ToString();
            var codeChallengeMethod = query["code_challenge_method"].ToString();
            var state = query["state"].ToString();
            var nonce = query["nonce"].ToString();

            var requestedScopes = scope.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var result = await mediator.Send(new AuthorizeQuery(
                clientId, redirectUri, responseType, requestedScopes, codeChallenge, codeChallengeMethod,
                NullIfEmpty(state), NullIfEmpty(nonce)), cancellationToken);

            if (!result.IsValid)
            {
                // An unrecognized client id or unregistered redirect location cannot be trusted with a
                // redirect (FR-007) — show a generic error directly instead.
                if (!result.CanRedirectToClient)
                {
                    return Results.BadRequest(new { error = result.ErrorCode, error_description = result.ErrorDescription });
                }

                var errorRedirect = QueryHelpers.AddQueryString(redirectUri, new Dictionary<string, string?>
                {
                    ["error"] = result.ErrorCode,
                    ["error_description"] = result.ErrorDescription,
                    ["state"] = NullIfEmpty(state)
                });

                return Results.Redirect(errorRedirect);
            }

            var html = LoginPage.Render(clientId, redirectUri, scope, codeChallenge, codeChallengeMethod, state, nonce, errorMessage: null);
            return Results.Content(html, "text/html");
        })
        .WithName("Authorize")
        .WithTags("OAuth2 / OIDC Protocol")
        .WithSummary("Starts the Authorization Code + PKCE flow.")
        .WithDescription("Validates the client, redirect location, and PKCE challenge, then renders the credential-entry page.");

        return app;
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;
}
