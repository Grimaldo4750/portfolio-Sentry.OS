using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentry.OS.Domain.Clients;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Persistence.Conventions;

namespace Sentry.OS.Persistence.Configurations;

/// <summary>EF configuration for <see cref="Client"/>.</summary>
public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Client> b)
    {
        b.ToTable("Clients", SchemaConventions.Authz);
        b.HasKey(x => x.Id);
        b.Property(x => x.ClientId).HasMaxLength(100).IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        b.Property(x => x.ClientSecretHash).HasMaxLength(200);
        b.HasIndex(x => x.ClientId).IsUnique();

        b.HasOne(x => x.Application)
            .WithMany(a => a.Clients)
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

/// <summary>EF configuration for <see cref="ClientRedirectUri"/>.</summary>
public class ClientRedirectUriConfiguration : IEntityTypeConfiguration<ClientRedirectUri>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ClientRedirectUri> b)
    {
        b.ToTable("ClientRedirectUris", SchemaConventions.Authz);
        b.HasKey(x => x.Id);
        b.Property(x => x.Uri).HasMaxLength(2000).IsRequired();
        b.HasIndex(x => new { x.ClientId, x.Uri }).IsUnique();

        b.HasOne(x => x.Client)
            .WithMany(c => c.RedirectUris)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>EF configuration for <see cref="ClientCorsOrigin"/>.</summary>
public class ClientCorsOriginConfiguration : IEntityTypeConfiguration<ClientCorsOrigin>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ClientCorsOrigin> b)
    {
        b.ToTable("ClientCorsOrigins", SchemaConventions.Authz);
        b.HasKey(x => x.Id);
        b.Property(x => x.Origin).HasMaxLength(500).IsRequired();
        b.HasIndex(x => new { x.ClientId, x.Origin }).IsUnique();

        b.HasOne(x => x.Client)
            .WithMany(c => c.CorsOrigins)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>EF configuration for <see cref="ClientGrantType"/>.</summary>
public class ClientGrantTypeConfiguration : IEntityTypeConfiguration<ClientGrantType>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ClientGrantType> b)
    {
        b.ToTable("ClientGrantTypes", SchemaConventions.Authz);
        b.HasKey(x => x.Id);
        b.Property(x => x.GrantType).HasMaxLength(50).IsRequired();
        b.HasIndex(x => new { x.ClientId, x.GrantType }).IsUnique();

        b.HasOne(x => x.Client)
            .WithMany(c => c.GrantTypes)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>EF configuration for the <see cref="ClientAllowedScope"/> join.</summary>
public class ClientAllowedScopeConfiguration : IEntityTypeConfiguration<ClientAllowedScope>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ClientAllowedScope> b)
    {
        b.ToTable("ClientAllowedScopes", SchemaConventions.Authz);
        b.HasKey(x => new { x.ClientId, x.ScopeId });

        b.HasOne(x => x.Client)
            .WithMany(c => c.AllowedScopes)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Scope)
            .WithMany(s => s.ClientAllowedScopes)
            .HasForeignKey(x => x.ScopeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
