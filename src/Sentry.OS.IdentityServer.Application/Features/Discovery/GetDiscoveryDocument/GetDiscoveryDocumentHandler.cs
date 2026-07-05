using MediatR;
using Sentry.OS.IdentityServer.Application.Common;

namespace Sentry.OS.IdentityServer.Application.Features.Discovery.GetDiscoveryDocument;

public class GetDiscoveryDocumentHandler(IIdentityServerOptions options)
    : IRequestHandler<GetDiscoveryDocumentQuery, GetDiscoveryDocumentResponse>
{
    public Task<GetDiscoveryDocumentResponse> Handle(GetDiscoveryDocumentQuery request, CancellationToken cancellationToken)
    {
        var issuer = options.Issuer;

        return Task.FromResult(new GetDiscoveryDocumentResponse(
            Issuer: issuer,
            AuthorizationEndpoint: $"{issuer}/connect/authorize",
            TokenEndpoint: $"{issuer}/connect/token",
            UserInfoEndpoint: $"{issuer}/connect/userinfo",
            JwksUri: $"{issuer}/.well-known/jwks.json",
            RevocationEndpoint: $"{issuer}/connect/revocation",
            ResponseTypesSupported: ["code"],
            GrantTypesSupported: ["authorization_code", "refresh_token"],
            IdTokenSigningAlgValuesSupported: ["RS256"],
            CodeChallengeMethodsSupported: ["S256"]));
    }
}
