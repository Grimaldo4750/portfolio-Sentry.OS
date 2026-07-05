using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentry.OS.Domain.Users;
using Sentry.OS.Persistence.Conventions;

namespace Sentry.OS.Persistence.Configurations;

/// <summary>EF configuration for <see cref="User"/> (platform-global, no tenant scope).</summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users", SchemaConventions.Identity);
        b.HasKey(x => x.Id);

        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.NormalizedEmail).HasMaxLength(256).IsRequired();
        b.Property(x => x.UserName).HasMaxLength(256).IsRequired();
        b.Property(x => x.PasswordHash).HasMaxLength(400).IsRequired();
        b.Property(x => x.SecurityStamp).HasMaxLength(64).IsRequired();
        b.Property(x => x.FirstName).HasMaxLength(100);
        b.Property(x => x.LastName).HasMaxLength(100);
        b.Property(x => x.ProfilePictureUrl).HasMaxLength(1000);
        b.Property(x => x.TwoFactorMethod).HasMaxLength(20);
        b.Property(x => x.PhoneNumber).HasMaxLength(30);

        b.Property(x => x.IsGlobalAdministrator).HasDefaultValue(false);

        b.HasIndex(x => x.NormalizedEmail).IsUnique();
        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.UserName).IsUnique();

        b.HasOne(x => x.ProfilePicture)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfilePicture>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
