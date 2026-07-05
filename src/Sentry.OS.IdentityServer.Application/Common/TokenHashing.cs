using System.Security.Cryptography;
using System.Text;

namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>
/// Hashes high-entropy, single-use secrets (refresh tokens, email-verification tokens, two-factor
/// codes) for storage. Unlike passwords, these values are generated (not user-chosen) and compared
/// by exact hash lookup, so a per-value salt is unnecessary — a plain SHA-256 digest is sufficient
/// and lets the store look the value up directly by hash.
/// </summary>
public static class TokenHashing
{
    public static string Hash(string value) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
