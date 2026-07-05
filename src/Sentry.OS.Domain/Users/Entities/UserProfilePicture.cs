namespace Sentry.OS.Domain.Users;

/// <summary>
/// Optional inline binary profile picture stored 1:1 with a user in a separate table so the
/// frequently-read <see cref="User"/> row stays lean.
/// </summary>
public class UserProfilePicture
{
    /// <summary>Shared primary key / foreign key to the owning user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Raw image bytes.</summary>
    public byte[] Content { get; set; } = [];

    /// <summary>MIME content type (e.g. <c>image/png</c>).</summary>
    public string ContentType { get; set; } = null!;

    /// <summary>Size of the content in bytes.</summary>
    public int SizeBytes { get; set; }

    /// <summary>UTC upload timestamp.</summary>
    public DateTime UploadedAtUtc { get; set; }

    /// <summary>Navigation to the owning user.</summary>
    public User User { get; set; } = null!;
}
