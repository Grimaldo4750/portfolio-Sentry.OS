using System.Security.Cryptography;
using System.Text;

namespace Sentry.OS.IdentityServer.Application.Common.Security;

/// <summary>
/// Verifies a PKCE (RFC 7636) <c>code_verifier</c> against the <c>code_challenge</c> captured at
/// authorization time. <c>S256</c> is the only supported transform; <c>plain</c> is rejected since
/// the seeded client always requires PKCE and S256 is the only OAuth-recommended method. Pure BCL
/// cryptography with no infrastructure dependency, so it lives directly in Application.
/// </summary>
public class PkceValidator
{
    public bool Verify(string codeVerifier, string codeChallenge, string codeChallengeMethod)
    {
        if (string.IsNullOrEmpty(codeVerifier) || string.IsNullOrEmpty(codeChallenge))
        {
            return false;
        }

        if (!string.Equals(codeChallengeMethod, "S256", StringComparison.Ordinal))
        {
            return false;
        }

        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        var computedChallenge = Base64UrlEncode(hash);

        var computedBytes = Encoding.ASCII.GetBytes(computedChallenge);
        var providedBytes = Encoding.ASCII.GetBytes(codeChallenge);

        return CryptographicOperations.FixedTimeEquals(computedBytes, providedBytes);
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
