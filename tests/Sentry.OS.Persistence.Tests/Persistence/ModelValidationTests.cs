using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Sentry.OS.Persistence;
using Sentry.OS.Persistence.Seed;

namespace Sentry.OS.Persistence.Tests.Persistence;

/// <summary>
/// Validates that the identity model builds and that the development seed is internally consistent.
/// These assertions inspect the EF model only; no database connection is made.
/// </summary>
public class ModelValidationTests
{
    private static IdentityDbContext CreateContext()
        => new IdentityDbContextFactory().CreateDbContext([]);

    // Seed data (HasData) lives on the design-time model, not the runtime read-optimized model.
    private static IModel DesignTimeModel(IdentityDbContext context)
        => context.GetService<IDesignTimeModel>().Model;

    [Fact]
    public void Model_Builds_With_All_Expected_Entities()
    {
        using var context = CreateContext();

        // 19 domain entity types are mapped (the migrations-history table is not a model entity).
        var mappedTypes = context.Model.GetEntityTypes().Select(t => t.ClrType.Name).ToHashSet();

        string[] expected =
        [
            "Organization", "OrganizationMembership", "User", "UserClaim", "UserToken", "UserProfilePicture",
            "Application", "Client", "ClientRedirectUri", "ClientCorsOrigin",
            "ClientGrantType", "ClientAllowedScope", "ApiResource", "Scope", "Role", "RoleScope",
            "RoleAssignment", "RefreshToken", "AuditLog"
        ];

        Assert.Equal(19, expected.Length);
        foreach (var type in expected)
        {
            Assert.Contains(type, mappedTypes);
        }
        Assert.Equal(19, context.Model.GetEntityTypes().Count());
    }

    [Fact]
    public void No_Permission_Group_Or_ApplicationClaim_Table_Is_Mapped()
    {
        using var context = CreateContext();

        var mappedTypes = context.Model.GetEntityTypes().Select(t => t.ClrType.Name).ToHashSet();

        // Constitution Principle VII: no Permission/Group/policy-engine/ApplicationClaim entity.
        Assert.DoesNotContain("Permission", mappedTypes);
        Assert.DoesNotContain("RolePermission", mappedTypes);
        Assert.DoesNotContain("ApplicationClaim", mappedTypes);
        Assert.DoesNotContain("Group", mappedTypes);
    }

    [Fact]
    public void OrganizationScoped_Tables_Expose_OrganizationId()
    {
        using var context = CreateContext();

        foreach (var entityType in context.Model.GetEntityTypes())
        {
            if (typeof(Sentry.OS.Domain.Common.IOrganizationScoped).IsAssignableFrom(entityType.ClrType))
            {
                Assert.NotNull(entityType.FindProperty("OrganizationId"));
            }
        }
    }

    [Fact]
    public void Role_Level_Is_Nullable_And_Administrative_Only()
    {
        using var context = CreateContext();

        var roleLevel = context.Model
            .FindEntityType(typeof(Sentry.OS.Domain.Authorization.Role))!
            .FindProperty(nameof(Sentry.OS.Domain.Authorization.Role.Level))!;

        Assert.True(roleLevel.IsNullable);
    }

    [Fact]
    public void Seed_Admin_Password_Is_Hashed_Not_Plaintext()
    {
        using var context = CreateContext();

        var userSeed = DesignTimeModel(context)
            .FindEntityType(typeof(Sentry.OS.Domain.Users.User))!
            .GetSeedData()
            .Single();

        var passwordHash = (string)userSeed["PasswordHash"]!;

        Assert.StartsWith("PBKDF2", passwordHash);
        Assert.DoesNotContain(SeedConstants.AdminPassword, passwordHash);
    }

    [Fact]
    public void Seed_Has_Exactly_One_Home_Organization_For_Admin()
    {
        using var context = CreateContext();

        var memberships = DesignTimeModel(context)
            .FindEntityType(typeof(Sentry.OS.Domain.Organizations.OrganizationMembership))!
            .GetSeedData();

        var homeCount = memberships.Count(m => (bool)m["IsHomeOrganization"]! &&
                                               (Guid)m["UserId"]! == SeedConstants.AdminUserId);

        Assert.Equal(1, homeCount);
    }

    [Fact]
    public void Seed_RoleScopes_Reference_Existing_Seed_Scopes()
    {
        using var context = CreateContext();

        var model = DesignTimeModel(context);

        var scopeIds = model
            .FindEntityType(typeof(Sentry.OS.Domain.Resources.Scope))!
            .GetSeedData()
            .Select(s => (Guid)s["Id"]!)
            .ToHashSet();

        var roleScopeScopeIds = model
            .FindEntityType(typeof(Sentry.OS.Domain.Authorization.RoleScope))!
            .GetSeedData()
            .Select(rs => (Guid)rs["ScopeId"]!);

        Assert.All(roleScopeScopeIds, id => Assert.Contains(id, scopeIds));
    }

    [Fact]
    public void Seed_Token_Scope_Intersection_Matches_Expected_Baseline_Scopes()
    {
        using var context = CreateContext();

        var model = DesignTimeModel(context);

        var roleScopeIds = model
            .FindEntityType(typeof(Sentry.OS.Domain.Authorization.RoleScope))!
            .GetSeedData()
            .Where(rs => (Guid)rs["RoleId"]! == SeedConstants.RoleId)
            .Select(rs => (Guid)rs["ScopeId"]!)
            .ToHashSet();

        var clientAllowedScopeIds = model
            .FindEntityType(typeof(Sentry.OS.Domain.Clients.ClientAllowedScope))!
            .GetSeedData()
            .Where(cas => (Guid)cas["ClientId"]! == SeedConstants.ClientId)
            .Select(cas => (Guid)cas["ScopeId"]!)
            .ToHashSet();

        var intersection = roleScopeIds.Intersect(clientAllowedScopeIds).OrderBy(id => id);
        var expected = new[] { SeedConstants.ScopeReadId, SeedConstants.ScopeWriteId }.OrderBy(id => id);

        Assert.Equal(expected, intersection);
    }
}
