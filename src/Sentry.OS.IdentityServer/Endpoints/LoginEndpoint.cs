using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Sentry.OS.IdentityServer.Application.Features.Authentication.SignIn;
using Sentry.OS.IdentityServer.Pages;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps <c>POST /connect/login</c> — the credential-entry step posted from <see cref="LoginPage"/>.</summary>
public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/connect/login", async (HttpRequest httpRequest, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var form = await httpRequest.ReadFormAsync(cancellationToken);
            var clientId = form["client_id"].ToString();
            var redirectUri = form["redirect_uri"].ToString();
            var scope = form["scope"].ToString();
            var codeChallenge = form["code_challenge"].ToString();
            var codeChallengeMethod = form["code_challenge_method"].ToString();
            var state = NullIfEmpty(form["state"].ToString());
            var nonce = NullIfEmpty(form["nonce"].ToString());
            var email = form["email"].ToString();
            var password = form["password"].ToString();

            var requestedScopes = scope.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var result = await mediator.Send(
                new SignInCommand(email, password, clientId, redirectUri, requestedScopes, codeChallenge, codeChallengeMethod, state, nonce),
                cancellationToken);

            if (result.TwoFactorRequired)
            {
                var html = TwoFactorPage.Render(
                    result.PendingUserId!.Value, clientId, redirectUri, scope, codeChallenge, codeChallengeMethod, state, nonce,
                    message: "We emailed you a verification code.");
                return Results.Content(html, "text/html");
            }

            if (!result.Succeeded)
            {
                var html = LoginPage.Render(clientId, redirectUri, scope, codeChallenge, codeChallengeMethod, state, nonce,
                    errorMessage: "Invalid email or password.");
                return Results.Content(html, "text/html");
            }

            var redirect = QueryHelpers.AddQueryString(redirectUri, new Dictionary<string, string?>
            {
                ["code"] = result.AuthorizationCode,
                ["state"] = state
            });

            return Results.Redirect(redirect);
        })
        .WithName("Login")
        .WithTags("Account")
        .WithSummary("Verifies the submitted credentials and continues the Authorization Code flow.")
        .RequireRateLimiting("auth");

        return app;
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;
}
