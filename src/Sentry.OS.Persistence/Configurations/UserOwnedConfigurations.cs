using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentry.OS.Domain.Users;
using Sentry.OS.Persistence.Conventions;

namespace Sentry.OS.Persistence.Configurations;

/// <summary>EF configuration for <see cref="UserClaim"/>.</summary>
public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserClaim> b)
    {
        b.ToTable("UserClaims", SchemaConventions.Identity);
        b.HasKey(x => x.Id);
        b.Property(x => x.ClaimType).HasMaxLength(200).IsRequired();
        b.Property(x => x.ClaimValue).HasMaxLength(1000).IsRequired();
        b.HasIndex(x => new { x.UserId, x.ClaimType, x.ClaimValue }).IsUnique();

        b.HasOne(x => x.User)
            .WithMany(u => u.Claims)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>EF configuration for <see cref="UserToken"/>.</summary>
public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserToken> b)
    {
        b.ToTable("UserTokens", SchemaConventions.Identity);
        b.HasKey(x => x.Id);
        b.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(30).IsRequired();
        b.Property(x => x.TokenHash).HasMaxLength(200).IsRequired();
        b.HasIndex(x => new { x.UserId, x.Purpose });

        b.HasOne(x => x.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>EF configuration for <see cref="UserProfilePicture"/> (1:1 with user, shared PK).</summary>
public class UserProfilePictureConfiguration : IEntityTypeConfiguration<UserProfilePicture>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserProfilePicture> b)
    {
        b.ToTable("UserProfilePictures", SchemaConventions.Identity);
        b.HasKey(x => x.UserId);
        b.Property(x => x.UserId).ValueGeneratedNever();
        b.Property(x => x.Content).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        // Relationship configured on the User side (UserConfiguration).
    }
}
