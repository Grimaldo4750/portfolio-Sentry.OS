using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Persistence.Conventions;

namespace Sentry.OS.Persistence.Configurations;

/// <summary>EF configuration for <see cref="Role"/>.</summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("Roles", SchemaConventions.Authz);
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000);
        b.HasIndex(x => new { x.OrganizationId, x.Name }).IsUnique();

        b.HasOne(x => x.Organization)
            .WithMany(o => o.Roles)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

/// <summary>EF configuration for the <see cref="RoleScope"/> join.</summary>
public class RoleScopeConfiguration : IEntityTypeConfiguration<RoleScope>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RoleScope> b)
    {
        b.ToTable("RoleScopes", SchemaConventions.Authz);
        b.HasKey(x => new { x.RoleId, x.ScopeId });

        b.HasOne(x => x.Role)
            .WithMany(r => r.RoleScopes)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Scope)
            .WithMany(s => s.RoleScopes)
            .HasForeignKey(x => x.ScopeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

/// <summary>EF configuration for the <see cref="RoleAssignment"/> (User &#8596; Role).</summary>
public class RoleAssignmentConfiguration : IEntityTypeConfiguration<RoleAssignment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RoleAssignment> b)
    {
        b.ToTable("RoleAssignments", SchemaConventions.Authz);
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();

        b.HasOne(x => x.User)
            .WithMany(u => u.RoleAssignments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasOne(x => x.Role)
            .WithMany(r => r.RoleAssignments)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
