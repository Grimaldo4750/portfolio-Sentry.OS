using System.Security.Cryptography;
using System.Text;
using Sentry.OS.IdentityServer.Application.Common.Security;

namespace Sentry.OS.IdentityServer.Tests.Security;

public class PkceValidatorTests
{
    private readonly PkceValidator _validator = new();

    private static string ChallengeFor(string verifier)
    {
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(verifier));
        return Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    [Fact]
    public void Verify_Succeeds_For_Matching_S256_Verifier_And_Challenge()
    {
        const string verifier = "a-sufficiently-long-random-code-verifier-value-1234567890";
        var challenge = ChallengeFor(verifier);

        Assert.True(_validator.Verify(verifier, challenge, "S256"));
    }

    [Fact]
    public void Verify_Fails_For_Mismatched_Verifier()
    {
        var challenge = ChallengeFor("original-verifier-value");

        Assert.False(_validator.Verify("a-different-verifier-value", challenge, "S256"));
    }

    [Fact]
    public void Verify_Fails_When_Verifier_Or_Challenge_Is_Missing()
    {
        Assert.False(_validator.Verify("", "some-challenge", "S256"));
        Assert.False(_validator.Verify("some-verifier", "", "S256"));
    }

    [Fact]
    public void Verify_Rejects_Plain_Method()
    {
        const string verifier = "plain-verifier-value";

        Assert.False(_validator.Verify(verifier, verifier, "plain"));
    }
}
