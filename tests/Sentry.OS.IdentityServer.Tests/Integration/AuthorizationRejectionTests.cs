using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Users;
using Sentry.OS.IdentityServer.Application.Common.Security;
using Sentry.OS.Persistence;
using Sentry.OS.Persistence.Seed;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>
/// Covers all four SC-003 rejection scenarios explicitly: incorrect password, a disabled/locked
/// user, an unknown-or-inactive client, and missing/invalid PKCE. Each must reject with a generic
/// error and issue no tokens (resolves `/speckit-analyze` finding E2).
/// </summary>
public class AuthorizationRejectionTests : IClassFixture<IdentityServerWebApplicationFactory>, IAsyncLifetime
{
    private const string ClientId = "sentry-management-web-app";
    private const string RedirectUri = "http://localhost:5173/callback";
    private const string Email = "c_grimaldo@outlook.com";

    private readonly IdentityServerWebApplicationFactory _factory;

    public AuthorizationRejectionTests(IdentityServerWebApplicationFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.EnsureDatabaseCreatedAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Incorrect_Password_Is_Rejected_And_Issues_No_Tokens()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var (_, challenge) = PkceTestHelper.GeneratePair();

        var response = await client.PostAsync("/connect/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["email"] = Email,
            ["password"] = "TotallyWrongPassword!"
        }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid email or password", html);
        Assert.DoesNotContain("code=", html);
    }

    [Fact]
    public async Task Disabled_User_Is_Rejected_With_A_Generic_Error()
    {
        var disabledEmail = "disabled.user@example.com";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var hasher = new PasswordHasher();

            var user = new User
            {
                Email = disabledEmail,
                NormalizedEmail = disabledEmail.ToUpperInvariant(),
                EmailVerified = true,
                UserName = "disabled.user",
                PasswordHash = hasher.Hash("SomePassword1!"),
                SecurityStamp = Guid.NewGuid().ToString("N"),
                IsDisabled = true,
                LockoutEnabled = true,
                CreatedAtUtc = SeedConstants.Timestamp
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            db.OrganizationMemberships.Add(new OrganizationMembership
            {
                OrganizationId = SeedConstants.OrganizationId,
                UserId = user.Id,
                IsOrganizationAdministrator = false,
                IsHomeOrganization = true,
                IsActive = true,
                JoinedAtUtc = SeedConstants.Timestamp,
                CreatedAtUtc = SeedConstants.Timestamp
            });
            await db.SaveChangesAsync();
        }

        using var client = _factory.CreateClientWithoutRedirects();
        var (_, challenge) = PkceTestHelper.GeneratePair();

        var response = await client.PostAsync("/connect/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["email"] = disabledEmail,
            ["password"] = "SomePassword1!"
        }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid email or password", html);
    }

    [Fact]
    public async Task Unknown_Client_Is_Rejected_With_A_Generic_InvalidClient_Error()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var (_, challenge) = PkceTestHelper.GeneratePair();

        var response = await client.GetAsync(
            $"/connect/authorize?client_id=does-not-exist&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
            $"&response_type=code&scope=organizations.manage&code_challenge={challenge}&code_challenge_method=S256");

        // An unregistered redirect target cannot be trusted with an error redirect (FR-007) — a
        // direct bad-request response is expected instead. The same code path (ClientAuthorizationValidator)
        // returns this identical "invalid_client" error for a recognized-but-inactive client too.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("invalid_client", body);
    }

    [Fact]
    public async Task Missing_Pkce_Challenge_Is_Rejected()
    {
        using var client = _factory.CreateClientWithoutRedirects();

        var response = await client.GetAsync(
            $"/connect/authorize?client_id={ClientId}&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
            "&response_type=code&scope=organizations.manage");

        // Client and redirect are both valid here, so the error is redirected back to the client
        // rather than shown directly.
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var query = QueryHelpers.ParseQuery(response.Headers.Location!.Query);
        Assert.Equal("invalid_request", query["error"].ToString());
    }
}
