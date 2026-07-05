using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentry.OS.Domain.Auditing;
using Sentry.OS.Persistence.Conventions;

namespace Sentry.OS.Persistence.Configurations;

/// <summary>EF configuration for <see cref="AuditLog"/> (append-only, no foreign keys).</summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("AuditLogs", SchemaConventions.Audit);
        b.HasKey(x => x.Id);
        b.Property(x => x.ActorDisplay).HasMaxLength(256);
        b.Property(x => x.Action).HasMaxLength(100).IsRequired();
        b.Property(x => x.TargetType).HasMaxLength(100);
        b.Property(x => x.IpAddress).HasMaxLength(64);
        b.HasIndex(x => new { x.OrganizationId, x.OccurredAtUtc });
        // Intentionally no foreign keys: the trail must survive downstream deletions.
    }
}
