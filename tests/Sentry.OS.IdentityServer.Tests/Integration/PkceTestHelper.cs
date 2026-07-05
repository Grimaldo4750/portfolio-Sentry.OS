using System.Security.Cryptography;
using System.Text;

namespace Sentry.OS.IdentityServer.Tests.Integration;

internal static class PkceTestHelper
{
    public static (string Verifier, string Challenge) GeneratePair()
    {
        var verifier = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(verifier));
        var challenge = Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return (verifier, challenge);
    }
}
