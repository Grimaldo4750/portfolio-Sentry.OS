using System.Net;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>
/// Verifies the fixed-window rate limiter (FR-033) on the authentication-sensitive
/// <c>/connect/token</c> endpoint: requests within the configured limit succeed (are not
/// throttled), and requests beyond it are rejected with <c>429</c>. Uses its own dedicated
/// factory instance with a deliberately small permit limit — the shared fixture used by the
/// functional flow tests keeps a generously high limit so they are never throttled by accident.
/// </summary>
public class RateLimitingTests : IAsyncLifetime
{
    private readonly IdentityServerWebApplicationFactory _factory = new() { RateLimitPermitLimit = 3 };

    public Task InitializeAsync() => _factory.EnsureDatabaseCreatedAsync();

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Requests_Beyond_The_Permit_Limit_Are_Throttled_With_429()
    {
        using var client = _factory.CreateClientWithoutRedirects();

        var statuses = new List<HttpStatusCode>();
        for (var i = 0; i < 5; i++)
        {
            // An intentionally-invalid grant is enough to exercise the endpoint's rate limiter
            // without needing a fresh authorization code per attempt.
            var response = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = "not-a-real-code",
                ["redirect_uri"] = "http://localhost:5173/callback",
                ["client_id"] = "sentry-management-web-app",
                ["code_verifier"] = "irrelevant"
            }));

            statuses.Add(response.StatusCode);
        }

        Assert.Equal(3, statuses.Count(s => s == HttpStatusCode.BadRequest));
        Assert.Equal(2, statuses.Count(s => s == HttpStatusCode.TooManyRequests));
    }
}
