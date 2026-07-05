using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sentry.OS.Admin.Infrastructure.Security;

/// <summary>Wires bearer-JWT validation against the IdentityServer's discovery/JWKS endpoint.</summary>
public static class AuthenticationServiceCollectionExtensions
{
    /// <summary>
    /// Registers JWT bearer authentication. This API only validates tokens issued by
    /// <c>Sentry.OS.IdentityServer</c> — it never issues, signs, or mints tokens itself.
    /// </summary>
    public static IServiceCollection AddAdminApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["IdentityServer:Authority"]
            ?? throw new InvalidOperationException("IdentityServer:Authority configuration value is required.");
        var audience = configuration["IdentityServer:Audience"];
        var requireHttpsMetadata = configuration.GetValue("IdentityServer:RequireHttpsMetadata", true);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = requireHttpsMetadata;
                options.TokenValidationParameters.ValidateAudience = !string.IsNullOrEmpty(audience);
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("GlobalAdministrator", policy =>
                policy.RequireClaim(CurrentActorAccessor.GlobalAdministratorClaimType, "true"));
        });

        return services;
    }
}
