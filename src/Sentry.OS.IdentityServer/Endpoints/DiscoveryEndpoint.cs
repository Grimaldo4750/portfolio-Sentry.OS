using MediatR;
using Sentry.OS.IdentityServer.Application.Features.Discovery.GetDiscoveryDocument;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps <c>GET /.well-known/openid-configuration</c> (FR-013).</summary>
public static class DiscoveryEndpoint
{
    public static IEndpointRouteBuilder MapDiscoveryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/openid-configuration", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetDiscoveryDocumentQuery(), cancellationToken);

            return Results.Ok(new
            {
                issuer = result.Issuer,
                authorization_endpoint = result.AuthorizationEndpoint,
                token_endpoint = result.TokenEndpoint,
                userinfo_endpoint = result.UserInfoEndpoint,
                jwks_uri = result.JwksUri,
                revocation_endpoint = result.RevocationEndpoint,
                response_types_supported = result.ResponseTypesSupported,
                grant_types_supported = result.GrantTypesSupported,
                id_token_signing_alg_values_supported = result.IdTokenSigningAlgValuesSupported,
                code_challenge_methods_supported = result.CodeChallengeMethodsSupported
            });
        })
        .WithName("Discovery")
        .WithTags("OAuth2 / OIDC Protocol")
        .WithSummary("Publishes the IdP's OIDC discovery document.");

        return app;
    }
}
