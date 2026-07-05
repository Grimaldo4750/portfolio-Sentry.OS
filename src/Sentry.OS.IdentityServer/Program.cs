using System.Reflection;
using System.Threading.RateLimiting;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Endpoints;
using Sentry.OS.IdentityServer.Infrastructure;
using Sentry.OS.IdentityServer.Infrastructure.Keys;
using Sentry.OS.Persistence;
using Sentry.OS.Persistence.Abstractions;
using Serilog;

var applicationAssembly = typeof(Sentry.OS.IdentityServer.Application.Common.Repositories.IAuthUserRepository).Assembly;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// The IdP authenticates users before any organization context exists, so it always reports no
// ambient organization; repositories that need organization scoping do so explicitly per call
// (see Sentry.OS.Persistence.Repositories.AuthUserRepository and friends).
builder.Services.AddScoped<ICurrentOrganization, DesignTimeCurrentOrganization>();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("SentryOsIdentity"))
        .ConfigureWarnings(w =>
            w.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning)));

builder.Services.AddIdentityServerInfrastructure();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

TypeAdapterConfig.GlobalSettings.Scan(applicationAssembly);
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("IdentityServerCorsPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Applied to sign-in, token exchange, and revocation only (FR-033) — discovery/JWKS/userinfo
    // are meant to be polled freely by relying parties validating tokens offline.
    var permitLimit = builder.Configuration.GetValue("RateLimiting:Authentication:PermitLimit", 20);
    var windowSeconds = builder.Configuration.GetValue("RateLimiting:Authentication:WindowSeconds", 60);

    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromSeconds(windowSeconds)
            }));
});

// The IdP validates its own access tokens for the userinfo endpoint (FR-016), directly against
// the in-process signing key rather than fetching its own JWKS over HTTP.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<SigningKeyProvider, IIdentityServerOptions>((jwtOptions, signingKeyProvider, idpOptions) =>
    {
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = idpOptions.Issuer,
            ValidAudience = idpOptions.DefaultAudience,
            IssuerSigningKey = signingKeyProvider.SigningCredentials.Key
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Sentry.OS Identity Server", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("IdentityServerCorsPolicy");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Sentry.OS Identity Server");

app.MapAuthorizeEndpoint();
app.MapLoginEndpoint();
app.MapTwoFactorEndpoint();
app.MapTokenEndpoint();
app.MapEmailVerificationEndpoint();
app.MapDiscoveryEndpoint();
app.MapJwksEndpoint();
app.MapRevocationEndpoint();
app.MapUserInfoEndpoint();

app.Run();

// Exposes the top-level program to WebApplicationFactory<Program> in integration tests.
public partial class Program;
