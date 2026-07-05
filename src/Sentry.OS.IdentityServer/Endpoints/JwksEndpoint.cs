using MediatR;
using Sentry.OS.IdentityServer.Application.Features.Discovery.GetJwks;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps <c>GET /.well-known/jwks.json</c> — public signing key material only (FR-014).</summary>
public static class JwksEndpoint
{
    public static IEndpointRouteBuilder MapJwksEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/jwks.json", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetJwksQuery(), cancellationToken);

            return Results.Ok(new
            {
                keys = result.Keys.Select(k => new
                {
                    kty = k.Kty,
                    use = k.Use,
                    kid = k.Kid,
                    alg = k.Alg,
                    n = k.N,
                    e = k.E
                })
            });
        })
        .WithName("Jwks")
        .WithTags("OAuth2 / OIDC Protocol")
        .WithSummary("Publishes the IdP's current public signing keys.");

        return app;
    }
}
