namespace Sentry.OS.Persistence.Seed;

/// <summary>
/// Fixed identifiers and values used by <see cref="IdentitySeed"/>. All GUIDs are constant so
/// <c>HasData</c> is deterministic and re-running the generated idempotent script never changes or
/// duplicates baseline rows.
/// </summary>
public static class SeedConstants
{
    /// <summary>Deterministic timestamp stamped on all seed rows.</summary>
    public static readonly DateTime Timestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Development-only admin password. NOT for production. The stored hash below is the PBKDF2
    /// hash of this password with a fixed salt (format: <c>PBKDF2.SHA256.&lt;iterations&gt;$salt$hash</c>).
    /// </summary>
    public const string AdminPassword = "Admin#12345";

    /// <summary>Precomputed, deterministic PBKDF2 hash of <see cref="AdminPassword"/>.</summary>
    public const string AdminPasswordHash =
        "PBKDF2.SHA256.100000$AQIDBAUGBwgJCgsMDQ4PEA==$sWL+DhI+SQS25GASsBG4DVnKPUL144v0nRCNQOhPk04=";

    /// <summary>Fixed security stamp for the seed admin.</summary>
    public const string AdminSecurityStamp = "SEEDSTAMP0000000000000000000000A";

    /// <summary>Administrative level assigned to the seeded <c>OrganizationAdmin</c> role.</summary>
    public const int OrganizationAdminRoleLevel = 100;

    public static readonly Guid OrganizationId = new("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AdminUserId = new("22222222-2222-2222-2222-222222222222");
    public static readonly Guid MembershipId = new("33333333-3333-3333-3333-333333333333");
    public static readonly Guid ApplicationId = new("44444444-4444-4444-4444-444444444444");
    public static readonly Guid ClientId = new("55555555-5555-5555-5555-555555555555");
    public static readonly Guid ApiResourceId = new("66666666-6666-6666-6666-666666666666");
    public static readonly Guid ScopeReadId = new("77777777-7777-7777-7777-777777777777");
    public static readonly Guid ScopeWriteId = new("88888888-8888-8888-8888-888888888888");
    public static readonly Guid RoleId = new("99999999-9999-9999-9999-999999999999");
    public static readonly Guid GrantCodeId = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    public static readonly Guid GrantRefreshId = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
    public static readonly Guid RedirectUriId = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
    public static readonly Guid CorsOriginId = new("cccccccc-cccc-cccc-cccc-ccccccccccc1");
    public static readonly Guid RoleAssignmentId = new("dddddddd-dddd-dddd-dddd-ddddddddddd1");
}
