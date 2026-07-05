using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Persistence.Conventions;

namespace Sentry.OS.Persistence.Configurations;

/// <summary>EF configuration for <see cref="OrganizationMembership"/>.</summary>
public class OrganizationMembershipConfiguration : IEntityTypeConfiguration<OrganizationMembership>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OrganizationMembership> b)
    {
        b.ToTable("OrganizationMemberships", SchemaConventions.Identity);
        b.HasKey(x => x.Id);

        b.HasIndex(x => new { x.OrganizationId, x.UserId }).IsUnique();

        // At most one home organization per user.
        b.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("[IsHomeOrganization] = 1")
            .HasDatabaseName("UX_OrganizationMemberships_HomeOrganization_PerUser");

        b.HasOne(x => x.User)
            .WithMany(u => u.Memberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasOne(x => x.Organization)
            .WithMany(o => o.Memberships)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
