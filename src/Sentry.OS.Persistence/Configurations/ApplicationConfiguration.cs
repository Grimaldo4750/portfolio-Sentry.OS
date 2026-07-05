using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentry.OS.Domain.Applications;
using Sentry.OS.Persistence.Conventions;

namespace Sentry.OS.Persistence.Configurations;

/// <summary>EF configuration for <see cref="Application"/>.</summary>
public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Application> b)
    {
        b.ToTable("Applications", SchemaConventions.Identity);
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Slug).HasMaxLength(100).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000);
        b.HasIndex(x => new { x.OrganizationId, x.Slug }).IsUnique();

        b.HasOne(x => x.Organization)
            .WithMany(o => o.Applications)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
