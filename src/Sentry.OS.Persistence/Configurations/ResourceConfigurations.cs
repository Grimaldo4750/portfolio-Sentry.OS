using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Resources;
using Sentry.OS.Persistence.Conventions;

namespace Sentry.OS.Persistence.Configurations;

/// <summary>EF configuration for <see cref="ApiResource"/>.</summary>
public class ApiResourceConfiguration : IEntityTypeConfiguration<ApiResource>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApiResource> b)
    {
        b.ToTable("ApiResources", SchemaConventions.Authz);
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        b.HasIndex(x => new { x.ApplicationId, x.Name }).IsUnique();

        b.HasOne(x => x.Application)
            .WithMany(a => a.ApiResources)
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

/// <summary>EF configuration for <see cref="Scope"/>.</summary>
public class ScopeConfiguration : IEntityTypeConfiguration<Scope>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Scope> b)
    {
        b.ToTable("Scopes", SchemaConventions.Authz);
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000);
        b.HasIndex(x => new { x.ApiResourceId, x.Name }).IsUnique();

        b.HasOne(x => x.ApiResource)
            .WithMany(r => r.Scopes)
            .HasForeignKey(x => x.ApiResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
