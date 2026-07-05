using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>A valid access token returns the signed-in user's profile (FR-016); an invalid/missing token is rejected.</summary>
public class UserInfoTests : IClassFixture<IdentityServerWebApplicationFactory>, IAsyncLifetime
{
    private const string ClientId = "sentry-management-web-app";
    private const string RedirectUri = "http://localhost:5173/callback";
    private const string Email = "c_grimaldo@outlook.com";
    private const string Password = "D@ngerdays4750";

    private readonly IdentityServerWebApplicationFactory _factory;

    public UserInfoTests(IdentityServerWebApplicationFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.EnsureDatabaseCreatedAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<string> GetAccessTokenAsync(HttpClient client)
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
        return json.RootElement.GetProperty("access_token").GetString()!;
    }

    [Fact]
    public async Task Valid_Token_Returns_The_Signed_In_Users_Profile()
    {
        using var authClient = _factory.CreateClientWithoutRedirects();
        var accessToken = await GetAccessTokenAsync(authClient);

        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("/connect/userinfo");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(Email, json.RootElement.GetProperty("email").GetString());
        Assert.Equal("Christian Grimaldo", json.RootElement.GetProperty("name").GetString());
        Assert.True(json.RootElement.GetProperty("email_verified").GetBoolean());
    }

    [Fact]
    public async Task Missing_Token_Is_Rejected()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/connect/userinfo");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_Token_Is_Rejected()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-real-token");

        var response = await client.GetAsync("/connect/userinfo");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
