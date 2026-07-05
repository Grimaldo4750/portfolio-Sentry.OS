using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sentry.OS.IdentityServer.Application.Common;

namespace Sentry.OS.IdentityServer.Infrastructure.Keys;

/// <summary>
/// Manages the RSA key used to sign access/identity tokens and exposes its public half as a JSON Web
/// Key Set for <c>/.well-known/jwks.json</c>. In development, the key is loaded from a configured path
/// or generated once and persisted to a gitignored local PEM file so repeated restarts sign with the
/// same key; a real deployment supplies <c>Signing:KeyPath</c> pointing at an externally managed key
/// (Principle IX: signing keys are never committed to source control).
/// </summary>
public class SigningKeyProvider : IJwksProvider
{
    /// <summary>The credentials currently used to sign newly issued tokens.</summary>
    public SigningCredentials SigningCredentials { get; }

    /// <summary>Every currently-published public key (supports rotation: old keys stay verifiable after a new one starts signing).</summary>
    public IReadOnlyList<JsonWebKey> PublicJsonWebKeys { get; }

    public SigningKeyProvider(IConfiguration configuration, ILogger<SigningKeyProvider> logger)
    {
        var keyPath = configuration["Signing:KeyPath"]
            ?? Path.Combine(AppContext.BaseDirectory, "App_Data", "signing-key.pem");

        var rsa = LoadOrGenerateKey(keyPath, logger);
        var kid = ComputeKeyId(rsa);

        var securityKey = new RsaSecurityKey(rsa) { KeyId = kid };
        SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        var publicJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsa.ExportParameters(false)) { KeyId = kid });
        publicJwk.Use = "sig";
        publicJwk.Alg = SecurityAlgorithms.RsaSha256;
        PublicJsonWebKeys = [publicJwk];
    }

    private static RSA LoadOrGenerateKey(string keyPath, ILogger logger)
    {
        if (File.Exists(keyPath))
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(keyPath));
            return rsa;
        }

        var generated = RSA.Create(2048);
        var directory = Path.GetDirectoryName(keyPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(keyPath, generated.ExportPkcs8PrivateKeyPem());
        logger.LogInformation("Generated a new development IdP signing key at {KeyPath}", keyPath);

        return generated;
    }

    private static string ComputeKeyId(RSA rsa)
    {
        var publicKeyBytes = rsa.ExportRSAPublicKey();
        var hash = SHA256.HashData(publicKeyBytes);
        return Base64UrlEncoder.Encode(hash[..16]);
    }

    /// <inheritdoc />
    public IReadOnlyList<JsonWebKeyData> GetPublicKeys() =>
        PublicJsonWebKeys
            .Select(jwk => new JsonWebKeyData(jwk.Kty, jwk.Use, jwk.Kid, jwk.Alg, jwk.N, jwk.E))
            .ToList();
}
