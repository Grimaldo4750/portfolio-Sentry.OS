using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Infrastructure.Keys;
using Sentry.OS.IdentityServer.Infrastructure.Tokens;

namespace Sentry.OS.IdentityServer.Tests.Tokens;

public class JwtTokenServiceTests : IDisposable
{
    private readonly string _keyPath = Path.Combine(Path.GetTempPath(), $"jwt-test-key-{Guid.NewGuid():N}.pem");
    private readonly JwtTokenService _service;

    public JwtTokenServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Signing:KeyPath"] = _keyPath })
            .Build();

        var signingKeyProvider = new SigningKeyProvider(configuration, NullLogger<SigningKeyProvider>.Instance);
        var options = new FakeIdentityServerOptions();

        _service = new JwtTokenService(signingKeyProvider, options, TimeProvider.System);
    }

    public void Dispose()
    {
        if (File.Exists(_keyPath))
        {
            File.Delete(_keyPath);
        }
    }

    [Fact]
    public void CreateAccessToken_Composes_The_Exact_Claims_AdminApi_Reads()
    {
        var userId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        var result = _service.CreateAccessToken(
            userId, "sentry-management-web-app", organizationId, isGlobalAdministrator: true,
            roleNames: ["GlobalAdministrator"], administrativeRoleLevels: [100],
            grantedScopes: ["organizations.manage", "users.manage"], audience: "api-sentry-management",
            lifetimeSeconds: 3600);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

        Assert.Equal(userId.ToString(), jwt.Subject);
        Assert.Equal("api-sentry-management", jwt.Audiences.Single());
        Assert.Equal(organizationId.ToString(), jwt.Claims.Single(c => c.Type == "organization_id").Value);
        Assert.Equal("true", jwt.Claims.Single(c => c.Type == "global_administrator").Value);
        Assert.Equal("100", jwt.Claims.Single(c => c.Type == "role_level").Value);
        Assert.Equal("GlobalAdministrator", jwt.Claims.Single(c => c.Type == "role").Value);

        var scopeClaim = jwt.Claims.Single(c => c.Type == "scope").Value.Split(' ');
        Assert.Contains("organizations.manage", scopeClaim);
        Assert.Contains("users.manage", scopeClaim);
    }

    [Fact]
    public void CreateAccessToken_Omits_The_Scope_Claim_When_No_Scopes_Are_Granted()
    {
        var result = _service.CreateAccessToken(
            Guid.NewGuid(), "client", Guid.NewGuid(), isGlobalAdministrator: false,
            roleNames: [], administrativeRoleLevels: [], grantedScopes: [], audience: "api-sentry-management",
            lifetimeSeconds: 3600);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        Assert.DoesNotContain(jwt.Claims, c => c.Type == "scope");
    }

    [Fact]
    public void CreateIdentityToken_Describes_The_User_For_The_Requesting_Client()
    {
        var userId = Guid.NewGuid();

        var result = _service.CreateIdentityToken(
            userId, "sentry-management-web-app", "Christian Grimaldo", "c_grimaldo@outlook.com",
            emailVerified: true, authTimeUtc: DateTimeOffset.UtcNow, nonce: "abc123", lifetimeSeconds: 300);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

        Assert.Equal(userId.ToString(), jwt.Subject);
        Assert.Equal("sentry-management-web-app", jwt.Audiences.Single());
        Assert.Equal("Christian Grimaldo", jwt.Claims.Single(c => c.Type == "name").Value);
        Assert.Equal("c_grimaldo@outlook.com", jwt.Claims.Single(c => c.Type == "email").Value);
        Assert.Equal("true", jwt.Claims.Single(c => c.Type == "email_verified").Value);
        Assert.Equal("abc123", jwt.Claims.Single(c => c.Type == "nonce").Value);
    }

    private class FakeIdentityServerOptions : IIdentityServerOptions
    {
        public string Issuer => "https://localhost:5001";
        public string DefaultAudience => "api-sentry-management";
    }
}
