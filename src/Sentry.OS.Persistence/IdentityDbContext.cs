using Microsoft.EntityFrameworkCore;
using Sentry.OS.Persistence.Abstractions;
using Sentry.OS.Domain.Applications;
using Sentry.OS.Domain.Auditing;
using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Clients;
using Sentry.OS.Domain.Common;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Resources;
using Sentry.OS.Domain.Tokens;
using Sentry.OS.Domain.Users;
using Sentry.OS.Persistence.Seed;

namespace Sentry.OS.Persistence;

/// <summary>
/// The single canonical identity persistence context. Owns every identity entity, applies naming
/// conventions, organization-isolation and soft-delete global query filters, and the development
/// seed. This context never applies migrations at runtime (constitution Principle IV).
/// </summary>
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options, ICurrentOrganization currentOrganization)
    : DbContext(options)
{
    private readonly ICurrentOrganization _currentOrganization = currentOrganization;

    /// <summary>Ambient organization used by organization-isolation query filters.</summary>
    public Guid? CurrentOrganizationId => _currentOrganization.OrganizationId;

    // identity schema
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMembership> OrganizationMemberships => Set<OrganizationMembership>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserClaim> UserClaims => Set<UserClaim>();
    public DbSet<UserToken> UserTokens => Set<UserToken>();
    public DbSet<UserProfilePicture> UserProfilePictures => Set<UserProfilePicture>();
    public DbSet<Application> Applications => Set<Application>();

    // authz schema
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientRedirectUri> ClientRedirectUris => Set<ClientRedirectUri>();
    public DbSet<ClientCorsOrigin> ClientCorsOrigins => Set<ClientCorsOrigin>();
    public DbSet<ClientGrantType> ClientGrantTypes => Set<ClientGrantType>();
    public DbSet<ClientAllowedScope> ClientAllowedScopes => Set<ClientAllowedScope>();
    public DbSet<ApiResource> ApiResources => Set<ApiResource>();
    public DbSet<Scope> Scopes => Set<Scope>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RoleScope> RoleScopes => Set<RoleScope>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // audit schema
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        ApplyKeyAndConcurrencyConventions(modelBuilder);
        ApplyQueryFilters(modelBuilder);

        IdentitySeed.Apply(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>Sequential-GUID key defaults and rowversion concurrency tokens.</summary>
    private static void ApplyKeyAndConcurrencyConventions(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;

            var pk = entityType.FindPrimaryKey();
            if (pk is { Properties.Count: 1 } &&
                pk.Properties[0].Name == "Id" &&
                pk.Properties[0].ClrType == typeof(Guid))
            {
                modelBuilder.Entity(clr).Property("Id").HasDefaultValueSql("NEWSEQUENTIALID()");
            }

            if (typeof(AuditableEntity).IsAssignableFrom(clr))
            {
                modelBuilder.Entity(clr).Property(nameof(AuditableEntity.RowVersion)).IsRowVersion();
            }
        }
    }

    /// <summary>Soft-delete and organization-isolation global query filters.</summary>
    private void ApplyQueryFilters(ModelBuilder b)
    {
        // Soft-delete only (platform-global entities).
        b.Entity<Organization>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<User>().HasQueryFilter(e => !e.IsDeleted);

        // Soft-delete + organization isolation.
        b.Entity<Application>().HasQueryFilter(e => !e.IsDeleted && e.OrganizationId == CurrentOrganizationId);

        // Organization isolation only.
        b.Entity<OrganizationMembership>().HasQueryFilter(e => e.OrganizationId == CurrentOrganizationId);
        b.Entity<Client>().HasQueryFilter(e => e.OrganizationId == CurrentOrganizationId);
        b.Entity<ApiResource>().HasQueryFilter(e => e.OrganizationId == CurrentOrganizationId);
        b.Entity<Scope>().HasQueryFilter(e => e.OrganizationId == CurrentOrganizationId);
        b.Entity<Role>().HasQueryFilter(e => e.OrganizationId == CurrentOrganizationId);
        b.Entity<RoleAssignment>().HasQueryFilter(e => e.OrganizationId == CurrentOrganizationId);
        b.Entity<RefreshToken>().HasQueryFilter(e => e.OrganizationId == CurrentOrganizationId);
    }
}
