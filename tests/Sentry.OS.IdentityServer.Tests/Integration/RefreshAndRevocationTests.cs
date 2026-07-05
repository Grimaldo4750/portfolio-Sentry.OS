using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sentry.OS.Domain.Tokens;
using Sentry.OS.Persistence;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>
/// Refresh-token renewal with rotation and reuse detection (FR-009), and sign-out/revocation
/// (FR-010): a refresh renews without the password, a rotated (superseded) token is rejected on
/// reuse and invalidates its lineage, a revoked session's refresh token can no longer renew, and
/// an expired refresh token fails.
/// </summary>
public class RefreshAndRevocationTests : IClassFixture<IdentityServerWebApplicationFactory>, IAsyncLifetime
{
    private const string ClientId = "sentry-management-web-app";
    private const string RedirectUri = "http://localhost:5173/callback";
    private const string Email = "c_grimaldo@outlook.com";
    private const string Password = "D@ngerdays4750";

    private readonly IdentityServerWebApplicationFactory _factory;

    public RefreshAndRevocationTests(IdentityServerWebApplicationFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.EnsureDatabaseCreatedAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<string> SignInAndGetRefreshTokenAsync(HttpClient client)
    {
        var (verifier, challenge) = PkceTestHelper.GeneratePair();

        var loginResponse = await client.PostAsync("/connect/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["email"] = Email,
            ["password"] = Password
        }));

        var query = QueryHelpers.ParseQuery(loginResponse.Headers.Location!.Query);
        var code = query["code"].ToString();

        var tokenResponse = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = RedirectUri,
            ["client_id"] = ClientId,
            ["code_verifier"] = verifier
        }));

        using var json = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("refresh_token").GetString()!;
    }

    [Fact]
    public async Task Refresh_Renews_Tokens_Without_The_Password_And_Rotates_The_Refresh_Token()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var refreshToken = await SignInAndGetRefreshTokenAsync(client);

        var refreshResponse = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = ClientId
        }));

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        using var json = JsonDocument.Parse(await refreshResponse.Content.ReadAsStringAsync());
        var newAccessToken = json.RootElement.GetProperty("access_token").GetString();
        var newRefreshToken = json.RootElement.GetProperty("refresh_token").GetString();

        Assert.False(string.IsNullOrEmpty(newAccessToken));
        Assert.False(string.IsNullOrEmpty(newRefreshToken));
        Assert.NotEqual(refreshToken, newRefreshToken);
    }

    [Fact]
    public async Task Reusing_A_Superseded_Refresh_Token_Is_Rejected_And_Revokes_The_Lineage()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var originalRefreshToken = await SignInAndGetRefreshTokenAsync(client);

        // Rotate once — this consumes the original token.
        var firstRefresh = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = originalRefreshToken,
            ["client_id"] = ClientId
        }));
        using var firstJson = JsonDocument.Parse(await firstRefresh.Content.ReadAsStringAsync());
        var rotatedRefreshToken = firstJson.RootElement.GetProperty("refresh_token").GetString()!;

        // Reusing the superseded (original) token must be rejected.
        var reuseAttempt = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = originalRefreshToken,
            ["client_id"] = ClientId
        }));
        Assert.Equal(HttpStatusCode.BadRequest, reuseAttempt.StatusCode);

        // The reuse must have revoked the whole lineage — the legitimately-rotated token must
        // also no longer work.
        var rotatedAttempt = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = rotatedRefreshToken,
            ["client_id"] = ClientId
        }));
        Assert.Equal(HttpStatusCode.BadRequest, rotatedAttempt.StatusCode);
    }

    [Fact]
    public async Task Revoking_A_Session_Prevents_Further_Renewal()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var refreshToken = await SignInAndGetRefreshTokenAsync(client);

        var revokeResponse = await client.PostAsync("/connect/revocation", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["token"] = refreshToken
        }));
        Assert.Equal(HttpStatusCode.OK, revokeResponse.StatusCode);

        var refreshAfterRevoke = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = ClientId
        }));

        Assert.Equal(HttpStatusCode.BadRequest, refreshAfterRevoke.StatusCode);
    }

    [Fact]
    public async Task An_Expired_Refresh_Token_Fails_To_Renew()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var refreshToken = await SignInAndGetRefreshTokenAsync(client);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var hash = System.Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(refreshToken)));
            var token = await db.Set<RefreshToken>().IgnoreQueryFilters().SingleAsync(t => t.TokenHash == hash);
            token.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1);
            await db.SaveChangesAsync();
        }

        var refreshResponse = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = ClientId
        }));

        Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
    }
}
