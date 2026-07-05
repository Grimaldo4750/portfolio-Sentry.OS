using Microsoft.EntityFrameworkCore;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Common;
using Sentry.OS.Persistence;
using Sentry.OS.Persistence.Abstractions;

namespace Sentry.OS.Admin.Infrastructure.Persistence;

/// <summary>Stamps <see cref="AuditableEntity"/> create/modify metadata from the current actor and clock on every save.</summary>
public class AdminIdentityDbContext(
    DbContextOptions<AdminIdentityDbContext> options,
    ICurrentOrganization currentOrganization,
    ICurrentActor currentActor,
    TimeProvider timeProvider)
    : IdentityDbContext(options, currentOrganization)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampAuditableEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampAuditableEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void StampAuditableEntities()
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.CreatedBy = currentActor.UserId;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAtUtc = now;
                    entry.Entity.ModifiedBy = currentActor.UserId;
                    break;
            }
        }
    }
}
