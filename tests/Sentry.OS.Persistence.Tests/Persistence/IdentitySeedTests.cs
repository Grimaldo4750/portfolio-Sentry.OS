using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Sentry.OS.Domain.Applications;
using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Clients;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Resources;
using Sentry.OS.Domain.Users;
using Sentry.OS.Persistence.Seed;

namespace Sentry.OS.Persistence.Tests.Persistence;

/// <summary>
/// Verifies the minimal, single-record development seed (FR-018 through FR-030): exactly one of
/// each entity, correct global-administrator authority, correct timestamps, and a complete
/// seven-scope management role. These assertions inspect the design-time model's <c>HasData</c>
/// only (no database connection); the seed's re-application idempotency is guaranteed by the
/// generated idempotent SQL script's existence-checked <c>INSERT</c> guards
/// (<c>scripts/identity-schema.sql</c>, produced via <c>dotnet ef migrations script --idempotent</c>)
/// together with the fixed, constant-in-source identifiers below, which never change between runs.
/// </summary>
public class IdentitySeedTests
{
    private static IdentityDbContext CreateContext()
        => new IdentityDbContextFactory().CreateDbContext([]);

    private static IModel DesignTimeModel(IdentityDbContext context)
        => context.GetService<IDesignTimeModel>().Model;

    private static IReadOnlyList<IDictionary<string, object?>> SeedDataOf<TEntity>(IModel model)
        => model.FindEntityType(typeof(TEntity))!.GetSeedData().ToList();

    [Fact]
    public void Seed_Provisions_Exactly_One_Organization_Named_Acron()
    {
        using var context = CreateContext();
        var organizations = SeedDataOf<Organization>(DesignTimeModel(context));

        Assert.Single(organizations);
        Assert.Equal("Acron", organizations[0]["Name"]);
        Assert.True((bool)organizations[0]["IsActive"]!);
    }

    [Fact]
    public void Seed_Provisions_Exactly_One_User_As_Christian_Grimaldo_Global_Administrator()
    {
        using var context = CreateContext();
        var users = SeedDataOf<User>(DesignTimeModel(context));

        Assert.Single(users);
        var user = users[0];
        Assert.Equal("c_grimaldo@outlook.com", user["Email"]);
        Assert.Equal("Christian", user["FirstName"]);
        Assert.Equal("Grimaldo", user["LastName"]);
        Assert.True((bool)user["IsGlobalAdministrator"]!);
        Assert.StartsWith("PBKDF2", (string)user["PasswordHash"]!);
        Assert.DoesNotContain(SeedConstants.AdminPassword, (string)user["PasswordHash"]!);
    }

    [Fact]
    public void Seed_Provisions_Exactly_One_Application_One_ApiResource_One_Client()
    {
        using var context = CreateContext();
        var model = DesignTimeModel(context);

        var applications = SeedDataOf<Application>(model);
        var apiResources = SeedDataOf<ApiResource>(model);
        var clients = SeedDataOf<Client>(model);

        Assert.Single(applications);
        Assert.Equal("Sentry Management Web App", applications[0]["Name"]);

        Assert.Single(apiResources);
        Assert.Equal("api-sentry-management", apiResources[0]["Name"]);

        Assert.Single(clients);
        Assert.Equal("sentry-management-web-app", clients[0]["ClientId"]);
        Assert.True((bool)clients[0]["RequirePkce"]!);
    }

    [Fact]
    public void Seed_Provisions_Exactly_Seven_Scopes_Covering_Every_Admin_Management_Area()
    {
        using var context = CreateContext();
        var scopes = SeedDataOf<Scope>(DesignTimeModel(context));

        var names = scopes.Select(s => (string)s["Name"]!).OrderBy(n => n).ToList();
        var expected = new[]
        {
            "applications.manage", "audit.read", "clients.manage", "organizations.manage",
            "resources.manage", "roles.manage", "users.manage"
        }.OrderBy(n => n).ToList();

        Assert.Equal(expected, names);
    }

    [Fact]
    public void Seed_Provisions_Exactly_One_Role_With_The_User_Assigned_To_It()
    {
        using var context = CreateContext();
        var model = DesignTimeModel(context);

        var roles = SeedDataOf<Role>(model);
        var roleAssignments = SeedDataOf<RoleAssignment>(model);

        Assert.Single(roles);
        Assert.Single(roleAssignments);
        Assert.Equal(SeedConstants.RoleId, roleAssignments[0]["RoleId"]);
        Assert.Equal(SeedConstants.AdminUserId, roleAssignments[0]["UserId"]);
    }

    [Fact]
    public void Seed_Introduces_No_Records_Beyond_The_Minimal_Set_Required()
    {
        using var context = CreateContext();
        var model = DesignTimeModel(context);

        Assert.Single(SeedDataOf<Organization>(model));
        Assert.Single(SeedDataOf<User>(model));
        Assert.Single(SeedDataOf<OrganizationMembership>(model));
        Assert.Single(SeedDataOf<Application>(model));
        Assert.Single(SeedDataOf<ApiResource>(model));
        Assert.Single(SeedDataOf<Client>(model));
        Assert.Single(SeedDataOf<Role>(model));
        Assert.Single(SeedDataOf<RoleAssignment>(model));
        Assert.Equal(7, SeedDataOf<Scope>(model).Count);
    }

    [Fact]
    public void Every_Seeded_Record_Is_Timestamped_2026_07_05_Utc()
    {
        using var context = CreateContext();
        var model = DesignTimeModel(context);

        var expected = new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, SeedConstants.Timestamp);

        Assert.All(SeedDataOf<Organization>(model), row => Assert.Equal(expected, row["CreatedAtUtc"]));
        Assert.All(SeedDataOf<User>(model), row => Assert.Equal(expected, row["CreatedAtUtc"]));
        Assert.All(SeedDataOf<Application>(model), row => Assert.Equal(expected, row["CreatedAtUtc"]));
    }

    [Fact]
    public void Every_Seeded_Identifier_Is_A_Random_Guid_Not_A_Placeholder_Pattern()
    {
        // "Random" is verified structurally: none of the seeded ids follow the old repeating-digit
        // placeholder pattern (e.g. 11111111-1111-...), and each is a distinct, well-formed GUID.
        var ids = new[]
        {
            SeedConstants.OrganizationId, SeedConstants.AdminUserId, SeedConstants.MembershipId,
            SeedConstants.ApplicationId, SeedConstants.ClientId, SeedConstants.ApiResourceId,
            SeedConstants.RoleId, SeedConstants.RoleAssignmentId,
            SeedConstants.ScopeOrganizationsManageId, SeedConstants.ScopeApplicationsManageId,
            SeedConstants.ScopeResourcesManageId, SeedConstants.ScopeClientsManageId,
            SeedConstants.ScopeRolesManageId, SeedConstants.ScopeUsersManageId, SeedConstants.ScopeAuditReadId
        };

        Assert.Equal(ids.Length, ids.Distinct().Count());
        Assert.All(ids, id => Assert.DoesNotMatch("^([0-9a-f])\\1{7}-\\1{4}-\\1{4}-\\1{4}-\\1{12}$", id.ToString()));
    }
}
