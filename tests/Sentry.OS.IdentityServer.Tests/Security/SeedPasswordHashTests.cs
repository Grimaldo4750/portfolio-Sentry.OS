using Sentry.OS.IdentityServer.Application.Common.Security;
using Sentry.OS.Persistence.Seed;

namespace Sentry.OS.IdentityServer.Tests.Security;

/// <summary>Verifies the seeded credential hash actually matches the documented development password (FR-019).</summary>
public class SeedPasswordHashTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Seeded_Hash_Verifies_Against_The_Documented_Password()
    {
        Assert.True(_hasher.Verify(SeedConstants.AdminPassword, SeedConstants.AdminPasswordHash));
    }

    [Fact]
    public void Seeded_Hash_Rejects_An_Incorrect_Password()
    {
        Assert.False(_hasher.Verify("WrongPassword123!", SeedConstants.AdminPasswordHash));
    }

    [Fact]
    public void Documented_Seed_Password_Matches_The_Requested_Credential()
    {
        Assert.Equal("D@ngerdays4750", SeedConstants.AdminPassword);
    }
}
