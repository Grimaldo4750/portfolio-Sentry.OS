namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>The public fields of an RSA JSON Web Key, framework-agnostic so Application stays free of a JWT library dependency.</summary>
public record JsonWebKeyData(string Kty, string Use, string Kid, string Alg, string N, string E);
