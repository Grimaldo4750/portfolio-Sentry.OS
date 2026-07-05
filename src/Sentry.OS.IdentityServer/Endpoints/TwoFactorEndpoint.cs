using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Sentry.OS.IdentityServer.Application.Features.Authentication.TwoFactor.RequestTwoFactorCode;
using Sentry.OS.IdentityServer.Application.Features.Authentication.TwoFactor.VerifyTwoFactorCode;
using Sentry.OS.IdentityServer.Pages;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps the two-factor verification and resend steps posted from <see cref="TwoFactorPage"/> (FR-032).</summary>
public static class TwoFactorEndpoint
{
    public static IEndpointRouteBuilder MapTwoFactorEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/connect/login/two-factor", async (HttpRequest httpRequest, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var form = await httpRequest.ReadFormAsync(cancellationToken);
            var userId = Guid.Parse(form["user_id"].ToString());
            var clientId = form["client_id"].ToString();
            var redirectUri = form["redirect_uri"].ToString();
            var scope = form["scope"].ToString();
            var codeChallenge = form["code_challenge"].ToString();
            var codeChallengeMethod = form["code_challenge_method"].ToString();
            var state = NullIfEmpty(form["state"].ToString());
            var nonce = NullIfEmpty(form["nonce"].ToString());
            var code = form["code"].ToString();

            var requestedScopes = scope.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var result = await mediator.Send(
                new VerifyTwoFactorCodeCommand(userId, code, clientId, redirectUri, requestedScopes, codeChallenge, codeChallengeMethod, state, nonce),
                cancellationToken);

            if (!result.Succeeded)
            {
                var html = TwoFactorPage.Render(userId, clientId, redirectUri, scope, codeChallenge, codeChallengeMethod, state, nonce,
                    message: "The verification code is incorrect or has expired.");
                return Results.Content(html, "text/html");
            }

            var redirect = QueryHelpers.AddQueryString(redirectUri, new Dictionary<string, string?>
            {
                ["code"] = result.AuthorizationCode,
                ["state"] = state
            });

            return Results.Redirect(redirect);
        })
        .WithName("VerifyTwoFactorCode")
        .WithTags("Account")
        .WithSummary("Verifies the emailed two-factor code and continues the Authorization Code flow.")
        .RequireRateLimiting("auth");

        app.MapPost("/connect/login/two-factor/resend", async (HttpRequest httpRequest, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var form = await httpRequest.ReadFormAsync(cancellationToken);
            var userId = Guid.Parse(form["user_id"].ToString());
            var clientId = form["client_id"].ToString();
            var redirectUri = form["redirect_uri"].ToString();
            var scope = form["scope"].ToString();
            var codeChallenge = form["code_challenge"].ToString();
            var codeChallengeMethod = form["code_challenge_method"].ToString();
            var state = NullIfEmpty(form["state"].ToString());
            var nonce = NullIfEmpty(form["nonce"].ToString());

            await mediator.Send(new RequestTwoFactorCodeCommand(userId), cancellationToken);

            var html = TwoFactorPage.Render(userId, clientId, redirectUri, scope, codeChallenge, codeChallengeMethod, state, nonce,
                message: "A new verification code has been sent.");
            return Results.Content(html, "text/html");
        })
        .WithName("ResendTwoFactorCode")
        .WithTags("Account")
        .WithSummary("Issues a fresh two-factor code, invalidating the previous one.")
        .RequireRateLimiting("auth");

        return app;
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;
}
