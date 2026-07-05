using Sentry.OS.IdentityServer.Application.Common.Security;

namespace Sentry.OS.IdentityServer.Tests.Security;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_Then_Verify_RoundTrips_The_Correct_Password()
    {
        var hash = _hasher.Hash("D@ngerdays4750");

        Assert.True(_hasher.Verify("D@ngerdays4750", hash));
    }

    [Fact]
    public void Verify_Rejects_An_Incorrect_Password()
    {
        var hash = _hasher.Hash("D@ngerdays4750");

        Assert.False(_hasher.Verify("wrong-password", hash));
    }

    [Fact]
    public void Hash_Produces_The_Documented_SelfDescribing_Format()
    {
        var hash = _hasher.Hash("some-password");

        Assert.StartsWith("PBKDF2.SHA256.100000$", hash);
        Assert.Equal(3, hash.Split('$').Length);
    }

    [Fact]
    public void Hash_Never_Contains_The_Plaintext_Password()
    {
        const string password = "D@ngerdays4750";
        var hash = _hasher.Hash(password);

        Assert.DoesNotContain(password, hash);
    }
}
