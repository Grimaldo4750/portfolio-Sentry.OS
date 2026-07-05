namespace Sentry.OS.IdentityServer.Application.Features.Discovery.GetDiscoveryDocument;

public record GetDiscoveryDocumentResponse(
    string Issuer,
    string AuthorizationEndpoint,
    string TokenEndpoint,
    string UserInfoEndpoint,
    string JwksUri,
    string RevocationEndpoint,
    IReadOnlyList<string> ResponseTypesSupported,
    IReadOnlyList<string> GrantTypesSupported,
    IReadOnlyList<string> IdTokenSigningAlgValuesSupported,
    IReadOnlyList<string> CodeChallengeMethodsSupported);
