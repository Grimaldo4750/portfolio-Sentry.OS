using Microsoft.Extensions.Configuration;
using Sentry.OS.IdentityServer.Application.Common;

namespace Sentry.OS.IdentityServer.Infrastructure;

/// <inheritdoc cref="IIdentityServerOptions" />
public class IdentityServerOptions(IConfiguration configuration) : IIdentityServerOptions
{
    public string Issuer =>
        configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer configuration value is required.");

    public string DefaultAudience =>
        configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience configuration value is required.");
}
