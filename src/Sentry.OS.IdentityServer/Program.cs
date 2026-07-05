using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sentry.OS.Persistence.Abstractions;
using Sentry.OS.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Ambient organization resolution is implemented in a later feature; the design-time placeholder
// is registered for now so the persistence context can be constructed.
builder.Services.AddScoped<ICurrentOrganization, DesignTimeCurrentOrganization>();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("SentryOsIdentity"))
        .ConfigureWarnings(w =>
            w.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning)));

var app = builder.Build();

app.MapGet("/", () => "Sentry.OS Identity Server");

app.Run();
