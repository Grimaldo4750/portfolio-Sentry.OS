namespace Sentry.OS.Persistence.Seed;

/// <summary>
/// Fixed identifiers and values used by <see cref="IdentitySeed"/>. All GUIDs are constant so
/// <c>HasData</c> is deterministic and re-running the generated idempotent script never changes or
/// duplicates baseline rows. This seed provisions exactly one organization (Acron), one global
/// administrator (Christian Grimaldo), one application (Sentry Management Web App), one API
/// resource (api-sentry-management) with one scope per Admin API management area, and one OAuth
/// client — the minimal set needed to sign in and actually operate the platform.
/// </summary>
public static class SeedConstants
{
    /// <summary>Deterministic timestamp stamped on all seed rows — the date this seed was authored (UTC).</summary>
    public static readonly DateTime Timestamp = new(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Development-only seed password for Christian Grimaldo (c_grimaldo@outlook.com). NOT a
    /// production secret — this is an obvious development placeholder, documented here alongside
    /// the seed definition per FR-029. The stored hash below is the PBKDF2 hash of this password.
    /// </summary>
    public const string AdminPassword = "D@ngerdays4750";

    /// <summary>Precomputed, deterministic PBKDF2 hash of <see cref="AdminPassword"/>.</summary>
    public const string AdminPasswordHash =
        "PBKDF2.SHA256.100000$vZ9+wS/g0hfSNvcAAizprg==$1AAjcmDjarHZQNAyaJ5vJF6v1wur5LJ0yb52HAeXFVs=";

    /// <summary>Fixed security stamp for the seed administrator.</summary>
    public const string AdminSecurityStamp = "SEEDSTAMP11062f87a73b41f6a26e6d580aeb02a9";

    /// <summary>Administrative level assigned to the seeded management role.</summary>
    public const int ManagementRoleLevel = 100;

    public static readonly Guid OrganizationId = new("02ab59f7-88da-4a57-b351-eea5207f34b8");
    public static readonly Guid AdminUserId = new("e23b2eae-0a19-4e08-b752-282af674137a");
    public static readonly Guid MembershipId = new("fa1d0cb9-6f57-442d-bab0-7c43079cb7a8");
    public static readonly Guid ApplicationId = new("0b12880d-dc23-4f74-a28f-f71525390a9c");
    public static readonly Guid ClientId = new("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f");
    public static readonly Guid ApiResourceId = new("d642f40e-bbef-4f01-b75c-f3ab939b240f");
    public static readonly Guid RoleId = new("f76fd1c9-48d6-4381-81cf-290dc89caad7");
    public static readonly Guid RoleAssignmentId = new("e07b9119-aaa4-4d10-9026-5968402243ce");
    public static readonly Guid GrantCodeId = new("09717d58-4a26-4945-9020-3f44d409bcc0");
    public static readonly Guid GrantRefreshId = new("3ef56dae-8cb1-465c-a011-7c66054fc362");
    public static readonly Guid RedirectUriId = new("86d91a06-f0bc-4550-bff0-8538c99b538c");
    public static readonly Guid CorsOriginId = new("184172f9-0490-4bb0-906e-65a1bf1e9fb4");

    // One scope per Admin Management API area (src/Sentry.OS.Admin.API/Controllers/) — real,
    // working administrative capability, not a symbolic pair.
    public static readonly Guid ScopeOrganizationsManageId = new("be5eadc2-d9e1-4bf1-93c0-0e29f2016f92");
    public static readonly Guid ScopeApplicationsManageId = new("42a42a3e-d8d1-42d2-894e-9151e69b0c2e");
    public static readonly Guid ScopeResourcesManageId = new("b22ed850-e3ee-4831-8edb-9cb1b882a03c");
    public static readonly Guid ScopeClientsManageId = new("95bcf91b-28b0-494a-ac55-c0d9cd328298");
    public static readonly Guid ScopeRolesManageId = new("1c954386-ac5d-45cf-94ff-8595fdaccb76");
    public static readonly Guid ScopeUsersManageId = new("01ab320f-5bbc-4c68-a91d-e578b4501d75");
    public static readonly Guid ScopeAuditReadId = new("91710057-3024-42c6-8e53-f2a7958b9e00");
}
