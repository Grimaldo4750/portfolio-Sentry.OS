using MediatR;
using Sentry.OS.IdentityServer.Application.Features.Tokens.RevokeToken;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps <c>POST /connect/revocation</c> (RFC 7009) — FR-010.</summary>
public static class RevocationEndpoint
{
    public static IEndpointRouteBuilder MapRevocationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/connect/revocation", async (HttpRequest httpRequest, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var form = await httpRequest.ReadFormAsync(cancellationToken);
            var token = form["token"].ToString();

            await mediator.Send(new RevokeTokenCommand(token), cancellationToken);

            // RFC 7009: the endpoint responds with 200 regardless of whether the token was known,
            // already revoked, or invalid — it never reveals which case occurred.
            return Results.Ok();
        })
        .WithName("Revocation")
        .WithTags("OAuth2 / OIDC Protocol")
        .WithSummary("Revokes a refresh token and its rotation lineage (RFC 7009).")
        .RequireRateLimiting("auth");

        return app;
    }
}
