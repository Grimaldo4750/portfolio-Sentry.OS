using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>End-to-end authorize → login → token happy path, and single-use-code rejection (FR-002, FR-004, FR-005, FR-008).</summary>
public class AuthorizationCodeFlowTests : IClassFixture<IdentityServerWebApplicationFactory>, IAsyncLifetime
{
    private const string ClientId = "sentry-management-web-app";
    private const string RedirectUri = "http://localhost:5173/callback";
    private const string Email = "c_grimaldo@outlook.com";
    private const string Password = "D@ngerdays4750";

    private readonly IdentityServerWebApplicationFactory _factory;

    public AuthorizationCodeFlowTests(IdentityServerWebApplicationFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.EnsureDatabaseCreatedAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Full_AuthorizationCode_Flow_Issues_A_Correctly_Scoped_Token()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var (verifier, challenge) = PkceTestHelper.GeneratePair();

        var authorizeResponse = await client.GetAsync(
            $"/connect/authorize?client_id={ClientId}&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
            $"&response_type=code&scope=organizations.manage+users.manage&code_challenge={challenge}" +
            "&code_challenge_method=S256&state=xyz");

        Assert.Equal(HttpStatusCode.OK, authorizeResponse.StatusCode);
        var loginHtml = await authorizeResponse.Content.ReadAsStringAsync();
        Assert.Contains("Sign in", loginHtml);

        var loginForm = new Dictionary<string, string>
        {
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage users.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["state"] = "xyz",
            ["email"] = Email,
            ["password"] = Password
        };

        var loginResponse = await client.PostAsync("/connect/login", new FormUrlEncodedContent(loginForm));

        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);
        var location = loginResponse.Headers.Location!;
        Assert.StartsWith(RedirectUri, location.ToString());

        var query = QueryHelpers.ParseQuery(location.Query);
        var code = query["code"].ToString();
        Assert.False(string.IsNullOrEmpty(code));
        Assert.Equal("xyz", query["state"].ToString());

        var tokenForm = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code!,
            ["redirect_uri"] = RedirectUri,
            ["client_id"] = ClientId,
            ["code_verifier"] = verifier
        };

        var tokenResponse = await client.PostAsync("/connect/token", new FormUrlEncodedContent(tokenForm));
        Assert.Equal(HttpStatusCode.OK, tokenResponse.StatusCode);

        var body = await tokenResponse.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        var accessToken = json.RootElement.GetProperty("access_token").GetString();
        var idToken = json.RootElement.GetProperty("id_token").GetString();
        var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();

        Assert.False(string.IsNullOrEmpty(accessToken));
        Assert.False(string.IsNullOrEmpty(idToken));
        Assert.False(string.IsNullOrEmpty(refreshToken));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        Assert.Equal("api-sentry-management", jwt.Audiences.Single());
        Assert.Equal("https://localhost:5001", jwt.Issuer);
        Assert.Equal("true", jwt.Claims.Single(c => c.Type == "global_administrator").Value);
        Assert.Contains(jwt.Claims, c => c.Type == "organization_id");

        var grantedScopes = jwt.Claims.Single(c => c.Type == "scope").Value.Split(' ');
        Assert.Contains("organizations.manage", grantedScopes);
        Assert.Contains("users.manage", grantedScopes);

        // The authorization code is single-use (FR-008): a second exchange must fail.
        var secondAttempt = await client.PostAsync("/connect/token", new FormUrlEncodedContent(tokenForm));
        Assert.Equal(HttpStatusCode.BadRequest, secondAttempt.StatusCode);
    }
}
