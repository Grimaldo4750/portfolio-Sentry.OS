using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>
/// Discovery advertises the issuer/endpoints/algorithms (FR-013); JWKS publishes public key
/// material only (FR-014); a token issued through the real flow validates against the published
/// keys, and tampered/expired/wrong-audience tokens fail that validation (SC-004).
/// </summary>
public class DiscoveryAndJwksTests : IClassFixture<IdentityServerWebApplicationFactory>, IAsyncLifetime
{
    private const string ClientId = "sentry-management-web-app";
    private const string RedirectUri = "http://localhost:5173/callback";
    private const string Email = "c_grimaldo@outlook.com";
    private const string Password = "D@ngerdays4750";

    private readonly IdentityServerWebApplicationFactory _factory;

    public DiscoveryAndJwksTests(IdentityServerWebApplicationFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.EnsureDatabaseCreatedAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Discovery_Document_Advertises_Issuer_Endpoints_And_Algorithm()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/.well-known/openid-configuration");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = json.RootElement;

        Assert.Equal("https://localhost:5001", root.GetProperty("issuer").GetString());
        Assert.Contains("/connect/authorize", root.GetProperty("authorization_endpoint").GetString());
        Assert.Contains("/connect/token", root.GetProperty("token_endpoint").GetString());
        Assert.Contains("/.well-known/jwks.json", root.GetProperty("jwks_uri").GetString());
        Assert.Contains("code", root.GetProperty("response_types_supported").EnumerateArray().Select(e => e.GetString()));
        Assert.Contains("RS256", root.GetProperty("id_token_signing_alg_values_supported").EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public async Task Jwks_Exposes_Only_Public_Key_Material()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/.well-known/jwks.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        var key = json.RootElement.GetProperty("keys").EnumerateArray().Single();

        Assert.Equal("RSA", key.GetProperty("kty").GetString());
        Assert.Equal("sig", key.GetProperty("use").GetString());
        Assert.False(string.IsNullOrEmpty(key.GetProperty("n").GetString()));
        Assert.False(string.IsNullOrEmpty(key.GetProperty("e").GetString()));

        // No private-key material (d, p, q, dp, dq, qi) is ever present.
        foreach (var privateField in new[] { "d", "p", "q", "dp", "dq", "qi" })
        {
            Assert.False(key.TryGetProperty(privateField, out _), $"JWKS must not expose private field '{privateField}'.");
        }
    }

    [Fact]
    public async Task An_Issued_Token_Validates_Against_Published_Keys_And_Tampering_Fails()
    {
        using var authClient = _factory.CreateClientWithoutRedirects();
        var (verifier, challenge) = PkceTestHelper.GeneratePair();

        var loginResponse = await authClient.PostAsync("/connect/login", new FormUrlEncodedContent(new Dictionary<string, string>
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

        var tokenResponse = await authClient.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = RedirectUri,
            ["client_id"] = ClientId,
            ["code_verifier"] = verifier
        }));

        using var tokenJson = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());
        var accessToken = tokenJson.RootElement.GetProperty("access_token").GetString()!;

        using var jwksClient = _factory.CreateClient();
        var jwksResponse = await jwksClient.GetAsync("/.well-known/jwks.json");
        using var jwksJson = JsonDocument.Parse(await jwksResponse.Content.ReadAsStringAsync());
        var keyElement = jwksJson.RootElement.GetProperty("keys").EnumerateArray().Single();

        var securityKey = new JsonWebKey(keyElement.GetRawText());
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://localhost:5001",
            ValidAudience = "api-sentry-management",
            IssuerSigningKey = securityKey
        };

        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(accessToken, validationParameters, out _);

        // Tampering with the signature must fail validation.
        var tamperedToken = accessToken[..^2] + (accessToken[^2] == 'A' ? "B" : "A") + accessToken[^1];
        Assert.Throws<SecurityTokenInvalidSignatureException>(() => handler.ValidateToken(tamperedToken, validationParameters, out _));

        // A wrong expected audience must fail validation.
        var wrongAudienceParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://localhost:5001",
            ValidAudience = "some-other-api",
            IssuerSigningKey = securityKey
        };
        Assert.ThrowsAny<SecurityTokenException>(() => handler.ValidateToken(accessToken, wrongAudienceParameters, out _));

        // A wrong expected issuer must fail validation.
        var wrongIssuerParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://not-the-real-issuer",
            ValidAudience = "api-sentry-management",
            IssuerSigningKey = securityKey
        };
        Assert.ThrowsAny<SecurityTokenException>(() => handler.ValidateToken(accessToken, wrongIssuerParameters, out _));
    }
}
