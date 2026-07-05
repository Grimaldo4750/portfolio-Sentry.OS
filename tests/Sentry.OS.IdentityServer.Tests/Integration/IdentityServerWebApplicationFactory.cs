using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.Persistence;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>
/// Boots the real IdP host (<see cref="Program"/>) against an isolated EF Core InMemory database
/// (seeded via the same <c>HasData</c> the real SQL Server deployment uses) so integration tests
/// exercise the actual endpoint pipeline without requiring a live SQL Server instance.
/// </summary>
public class IdentityServerWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    /// <summary>Every email "sent" during this factory's lifetime — used to extract verification links/codes.</summary>
    public TestEmailSender EmailSender { get; } = new();

    /// <summary>
    /// Requests permitted per rate-limit window. Defaults generously high so functional flow tests
    /// sharing one factory instance (and therefore one rate-limiter state) never trip it by accident;
    /// <see cref="RateLimitingTests"/> sets this low on its own dedicated instance to verify throttling.
    /// </summary>
    public int RateLimitPermitLimit { get; init; } = 1000;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "https://localhost:5001",
                ["Jwt:Audience"] = "api-sentry-management",
                ["RateLimiting:Authentication:PermitLimit"] = RateLimitPermitLimit.ToString(),
                ["RateLimiting:Authentication:WindowSeconds"] = "60",
                ["Cors:AllowedOrigins:0"] = "http://localhost:5173"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove every EF Core service tied to IdentityDbContext (options, the context
            // registration itself, and the internal IDbContextOptionsConfiguration<T> markers) —
            // removing only DbContextOptions<T> leaves the SqlServer configuration action
            // registered, which EF then tries to apply alongside InMemory and fails with
            // "two providers registered".
            var toRemove = services
                .Where(d => d.ServiceType == typeof(IdentityDbContext)
                    || (d.ServiceType.IsGenericType && d.ServiceType.GetGenericArguments().Contains(typeof(IdentityDbContext))))
                .ToList();

            foreach (var descriptor in toRemove)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<IdentityDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailSender));
            if (emailDescriptor is not null)
            {
                services.Remove(emailDescriptor);
            }

            services.AddSingleton<IEmailSender>(EmailSender);
        });
    }

    /// <summary>Materializes the InMemory database and its seed data (idempotent per factory instance).</summary>
    public async Task EnsureDatabaseCreatedAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    /// <summary>An HttpClient configured not to auto-follow redirects, so tests can inspect the Location header.</summary>
    public HttpClient CreateClientWithoutRedirects() =>
        CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
}
