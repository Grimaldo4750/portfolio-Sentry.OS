namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>Verifies the mandated security response headers are present on every response (Principle IX).</summary>
public class SecurityHeadersTests : IClassFixture<IdentityServerWebApplicationFactory>, IAsyncLifetime
{
    private readonly IdentityServerWebApplicationFactory _factory;

    public SecurityHeadersTests(IdentityServerWebApplicationFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.EnsureDatabaseCreatedAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Every_Response_Carries_The_Mandated_Security_Headers()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/.well-known/openid-configuration");

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
        Assert.Equal("no-referrer", response.Headers.GetValues("Referrer-Policy").Single());
    }
}
