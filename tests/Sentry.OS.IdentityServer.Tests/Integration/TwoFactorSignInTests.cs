using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Users;
using Sentry.OS.IdentityServer.Application.Common.Security;
using Sentry.OS.Persistence;
using Sentry.OS.Persistence.Seed;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>
/// Email-based two-factor authentication (FR-032): a correct password alone does not complete
/// sign-in, the correct emailed code does, a wrong/expired code is rejected, and requesting a
/// resend invalidates the previous code so only the newest one verifies.
/// </summary>
public partial class TwoFactorSignInTests : IClassFixture<IdentityServerWebApplicationFactory>, IAsyncLifetime
{
    private const string ClientId = "sentry-management-web-app";
    private const string RedirectUri = "http://localhost:5173/callback";
    private const string TwoFactorEmail = "twofactor.user@example.com";
    private const string TwoFactorPassword = "SomePassword1!";

    private readonly IdentityServerWebApplicationFactory _factory;

    public TwoFactorSignInTests(IdentityServerWebApplicationFactory factory) => _factory = factory;

    public async Task InitializeAsync()
    {
        await _factory.EnsureDatabaseCreatedAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var hasher = new PasswordHasher();

        var user = new User
        {
            Email = TwoFactorEmail,
            NormalizedEmail = TwoFactorEmail.ToUpperInvariant(),
            EmailVerified = true,
            UserName = "twofactor.user",
            PasswordHash = hasher.Hash(TwoFactorPassword),
            SecurityStamp = Guid.NewGuid().ToString("N"),
            TwoFactorEnabled = true,
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

    public Task DisposeAsync() => Task.CompletedTask;

    [GeneratedRegex(@"sign-in code is (\d{6})")]
    private static partial Regex CodeRegex();

    private static string ExtractCode(string emailBody) => CodeRegex().Match(emailBody).Groups[1].Value;

    [Fact]
    public async Task Password_Alone_Does_Not_Complete_SignIn_And_Correct_Code_Does()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var (verifier, challenge) = PkceTestHelper.GeneratePair();

        var loginResponse = await client.PostAsync("/connect/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["email"] = TwoFactorEmail,
            ["password"] = TwoFactorPassword
        }));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var html = await loginResponse.Content.ReadAsStringAsync();
        Assert.Contains("verification code", html);
        Assert.DoesNotContain("code=", html.Replace("user_id", string.Empty));

        var email = Assert.Single(_factory.EmailSender.SentEmails, e => e.To == TwoFactorEmail);
        var code = ExtractCode(email.Body);
        Assert.False(string.IsNullOrEmpty(code));

        var userIdMatch = Regex.Match(html, "name=\"user_id\" value=\"([^\"]+)\"");
        var userId = userIdMatch.Groups[1].Value;

        var verifyResponse = await client.PostAsync("/connect/login/two-factor", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["user_id"] = userId,
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["code"] = code
        }));

        Assert.Equal(HttpStatusCode.Redirect, verifyResponse.StatusCode);
        var query = QueryHelpers.ParseQuery(verifyResponse.Headers.Location!.Query);
        Assert.False(string.IsNullOrEmpty(query["code"].ToString()));
    }

    [Fact]
    public async Task Wrong_Code_Is_Rejected()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var (_, challenge) = PkceTestHelper.GeneratePair();

        var loginResponse = await client.PostAsync("/connect/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["email"] = TwoFactorEmail,
            ["password"] = TwoFactorPassword
        }));

        var html = await loginResponse.Content.ReadAsStringAsync();
        var userId = Regex.Match(html, "name=\"user_id\" value=\"([^\"]+)\"").Groups[1].Value;

        var verifyResponse = await client.PostAsync("/connect/login/two-factor", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["user_id"] = userId,
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["code"] = "000000"
        }));

        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
        var body = await verifyResponse.Content.ReadAsStringAsync();
        Assert.Contains("incorrect or has expired", body);
    }

    [Fact]
    public async Task Resend_Invalidates_The_Previous_Code_So_Only_The_Newest_Verifies()
    {
        using var client = _factory.CreateClientWithoutRedirects();
        var (verifier, challenge) = PkceTestHelper.GeneratePair();

        var loginResponse = await client.PostAsync("/connect/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["email"] = TwoFactorEmail,
            ["password"] = TwoFactorPassword
        }));

        var loginHtml = await loginResponse.Content.ReadAsStringAsync();
        var userId = Regex.Match(loginHtml, "name=\"user_id\" value=\"([^\"]+)\"").Groups[1].Value;
        var firstEmail = _factory.EmailSender.SentEmails.Last(e => e.To == TwoFactorEmail);
        var firstCode = ExtractCode(firstEmail.Body);

        var resendResponse = await client.PostAsync("/connect/login/two-factor/resend", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["user_id"] = userId,
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256"
        }));

        Assert.Equal(HttpStatusCode.OK, resendResponse.StatusCode);
        var secondEmail = _factory.EmailSender.SentEmails.Last(e => e.To == TwoFactorEmail);
        var secondCode = ExtractCode(secondEmail.Body);

        var oldCodeAttempt = await client.PostAsync("/connect/login/two-factor", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["user_id"] = userId,
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["code"] = firstCode
        }));
        Assert.Contains("incorrect or has expired", await oldCodeAttempt.Content.ReadAsStringAsync());

        var newCodeAttempt = await client.PostAsync("/connect/login/two-factor", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["user_id"] = userId,
            ["client_id"] = ClientId,
            ["redirect_uri"] = RedirectUri,
            ["scope"] = "organizations.manage",
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["code"] = secondCode
        }));
        Assert.Equal(HttpStatusCode.Redirect, newCodeAttempt.StatusCode);
    }
}
