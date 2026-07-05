using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Sentry.OS.Persistence;

/// <summary>
/// Design-time factory so <c>dotnet ef</c> can build the model and generate migration scripts
/// without starting the web host. Used only by tooling; never at runtime.
/// </summary>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    /// <inheritdoc />
    public IdentityDbContext CreateDbContext(string[] args)
    {
        // Get the environment (default to Development if not set)
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";

        // Build configuration from appsettings.json files in the IdentityServer project
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Sentry.OS.IdentityServer"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .Build();

        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("SentryOsIdentity")
            ?? throw new InvalidOperationException("Connection string 'SentryOsIdentity' not found in configuration.");

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer(connectionString)
            .ConfigureWarnings(w =>
                w.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning))
            .Options;

        return new IdentityDbContext(options, new DesignTimeCurrentOrganization());
    }
}
