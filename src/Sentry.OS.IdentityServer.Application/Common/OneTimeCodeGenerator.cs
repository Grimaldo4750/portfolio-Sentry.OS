using System.Security.Cryptography;

namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>Generates high-entropy, single-use secrets for refresh tokens, two-factor codes, and email verification.</summary>
public static class OneTimeCodeGenerator
{
    /// <summary>A 6-digit numeric code, suitable for a user to read and type (two-factor authentication).</summary>
    public static string GenerateNumericCode() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

    /// <summary>A 32-byte, URL-safe random token, suitable for embedding in a link (email verification, refresh tokens).</summary>
    public static string GenerateUrlSafeToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
