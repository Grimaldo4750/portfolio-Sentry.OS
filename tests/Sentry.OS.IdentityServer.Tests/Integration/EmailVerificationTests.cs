using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sentry.OS.Domain.Users;
using Sentry.OS.IdentityServer.Application.Common.Security;
using Sentry.OS.Persistence;
using Sentry.OS.Persistence.Seed;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>
/// Email verification (FR-031): requesting issues a token delivered by email, confirming marks the
/// email verified, and a reused (already-consumed) token is rejected.
/// </summary>
public partial class EmailVerificationTests : IClassFixture<IdentityServerWebApplicationFactory>, IAsyncLifetime
{
    private const string UnverifiedEmail = "unverified.user@example.com";

    private readonly IdentityServerWebApplicationFactory _factory;

    public EmailVerificationTests(IdentityServerWebApplicationFactory factory) => _factory = factory;

    public async Task InitializeAsync()
    {
        await _factory.EnsureDatabaseCreatedAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        db.Users.Add(new User
        {
            Email = UnverifiedEmail,
            NormalizedEmail = UnverifiedEmail.ToUpperInvariant(),
            EmailVerified = false,
            UserName = "unverified.user",
            PasswordHash = new PasswordHasher().Hash("SomePassword1!"),
            SecurityStamp = Guid.NewGuid().ToString("N"),
            LockoutEnabled = true,
            CreatedAtUtc = SeedConstants.Timestamp
        });
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [GeneratedRegex(@"token=([^\s&]+)")]
    private static partial Regex TokenRegex();

    [Fact]
    public async Task Request_Then_Confirm_Marks_The_Email_Verified_And_Reuse_Is_Rejected()
    {
        using var client = _factory.CreateClient();

        var sendResponse = await client.PostAsJsonAsync("/account/email-verification/send", new { email = UnverifiedEmail });
        Assert.Equal(HttpStatusCode.OK, sendResponse.StatusCode);

        var sentEmail = Assert.Single(_factory.EmailSender.SentEmails, e => e.To == UnverifiedEmail);
        var token = TokenRegex().Match(sentEmail.Body).Groups[1].Value;
        Assert.False(string.IsNullOrEmpty(token));

        var confirmResponse = await client.GetAsync($"/account/email-verification/confirm?token={token}");
        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);
        var confirmBody = await confirmResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(confirmBody.GetProperty("succeeded").GetBoolean());

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var user = await db.Users.SingleAsync(u => u.Email == UnverifiedEmail);
            Assert.True(user.EmailVerified);
        }

        // Reusing the same (now-consumed) token must fail.
        var reuseResponse = await client.GetAsync($"/account/email-verification/confirm?token={token}");
        Assert.Equal(HttpStatusCode.BadRequest, reuseResponse.StatusCode);
    }
}
