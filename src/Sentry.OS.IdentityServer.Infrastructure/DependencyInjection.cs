using Microsoft.Extensions.DependencyInjection;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;
using Sentry.OS.IdentityServer.Application.Common.Security;
using Sentry.OS.IdentityServer.Infrastructure.Email;
using Sentry.OS.IdentityServer.Infrastructure.Keys;
using Sentry.OS.IdentityServer.Infrastructure.Tokens;
using Sentry.OS.Persistence.Repositories;

namespace Sentry.OS.IdentityServer.Infrastructure;

/// <summary>Registers every IdentityServer.Infrastructure service and the EF Core repository implementations.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddIdentityServerInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<PasswordHasher>();
        services.AddSingleton<PkceValidator>();
        services.AddSingleton<ScopeIntersectionResolver>();
        services.AddSingleton<SigningKeyProvider>();
        // Same singleton instance backs both roles: JwtTokenService signs with it, and the JWKS
        // endpoint publishes its public half, so token `kid` and JWKS `kid` always agree (T065).
        services.AddSingleton<IJwksProvider>(sp => sp.GetRequiredService<SigningKeyProvider>());
        services.AddSingleton<IIdentityServerOptions, IdentityServerOptions>();
        services.AddSingleton<IEmailSender, DevelopmentEmailSender>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IAuthorizationCodeStore, AuthorizationCodeStore>();

        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<IAuthClientRepository, AuthClientRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();

        return services;
    }
}
