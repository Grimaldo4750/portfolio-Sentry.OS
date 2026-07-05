using System.Security.Cryptography;

namespace Sentry.OS.IdentityServer.Application.Common.Security;

/// <summary>
/// Hashes and verifies passwords using PBKDF2-HMAC-SHA256, storing the result as the self-describing
/// string <c>PBKDF2.SHA256.&lt;iterations&gt;$&lt;base64 salt&gt;$&lt;base64 subkey&gt;</c> so the
/// iteration count and salt travel with the hash and can be rotated without a format change. Pure BCL
/// cryptography with no infrastructure dependency, so it lives directly in Application (no interface
/// indirection needed) alongside the handlers that call it.
/// </summary>
public class PasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSizeBytes = 16;
    private const int SubkeySizeBytes = 32;

    /// <summary>Hashes a plaintext password. The plaintext is never retained.</summary>
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var subkey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, SubkeySizeBytes);

        return $"PBKDF2.SHA256.{Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(subkey)}";
    }

    /// <summary>Verifies a plaintext password against a hash produced by <see cref="Hash"/>.</summary>
    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('$');
        if (parts.Length != 3)
        {
            return false;
        }

        var header = parts[0].Split('.');
        if (header.Length != 3 || header[0] != "PBKDF2" || header[1] != "SHA256" || !int.TryParse(header[2], out var iterations))
        {
            return false;
        }

        byte[] salt;
        byte[] expectedSubkey;
        try
        {
            salt = Convert.FromBase64String(parts[1]);
            expectedSubkey = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualSubkey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedSubkey.Length);

        return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
    }
}
