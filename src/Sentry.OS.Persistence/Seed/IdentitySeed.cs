using Microsoft.EntityFrameworkCore;
using Sentry.OS.Domain.Applications;
using Sentry.OS.Domain.Authorization;
using Sentry.OS.Domain.Clients;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Resources;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Persistence.Seed;

/// <summary>
/// Idempotent development seed emitted via <c>HasData</c> so baseline rows are part of the generated
/// migration script (Principle IV). Provisions exactly the minimal, real-and-functional record set
/// requested: one organization (Acron), one global-administrator user (Christian Grimaldo), one
/// application (Sentry Management Web App), one API resource (api-sentry-management) carrying one
/// scope per Admin Management API area, one role bundling all of them, and one OAuth client — no
/// additional organizations, users, applications, clients, resources, or permissions (FR-025).
/// </summary>
public static class IdentitySeed
{
    /// <summary>Registers seed data on the model.</summary>
    public static void Apply(ModelBuilder b)
    {
        var ts = SeedConstants.Timestamp;

        b.Entity<Organization>().HasData(new Organization
        {
            Id = SeedConstants.OrganizationId,
            Name = "Acron",
            Slug = "acron",
            DisplayName = "Acron",
            IsActive = true,
            CreatedAtUtc = ts
        });

        b.Entity<User>().HasData(new User
        {
            Id = SeedConstants.AdminUserId,
            Email = "c_grimaldo@outlook.com",
            NormalizedEmail = "C_GRIMALDO@OUTLOOK.COM",
            EmailVerified = true,
            UserName = "c_grimaldo",
            PasswordHash = SeedConstants.AdminPasswordHash,
            SecurityStamp = SeedConstants.AdminSecurityStamp,
            FirstName = "Christian",
            LastName = "Grimaldo",
            IsGlobalAdministrator = true,
            LockoutEnabled = true,
            CreatedAtUtc = ts
        });

        b.Entity<OrganizationMembership>().HasData(new OrganizationMembership
        {
            Id = SeedConstants.MembershipId,
            OrganizationId = SeedConstants.OrganizationId,
            UserId = SeedConstants.AdminUserId,
            IsOrganizationAdministrator = true,
            IsHomeOrganization = true,
            IsActive = true,
            JoinedAtUtc = ts,
            CreatedAtUtc = ts
        });

        b.Entity<Application>().HasData(new Application
        {
            Id = SeedConstants.ApplicationId,
            OrganizationId = SeedConstants.OrganizationId,
            Name = "Sentry Management Web App",
            Slug = "sentry-management-web-app",
            Description = "The single web application used to sign in and administer Sentry.OS.",
            IsActive = true,
            CreatedAtUtc = ts
        });

        b.Entity<Client>().HasData(new Client
        {
            Id = SeedConstants.ClientId,
            OrganizationId = SeedConstants.OrganizationId,
            ApplicationId = SeedConstants.ApplicationId,
            ClientId = "sentry-management-web-app",
            DisplayName = "Sentry Management Web App (SPA)",
            ClientSecretHash = null,
            RequirePkce = true,
            RequireClientSecret = false,
            AccessTokenLifetimeSeconds = 3600,
            IdentityTokenLifetimeSeconds = 300,
            RefreshTokenLifetimeSeconds = 1209600,
            RefreshTokenRotationEnabled = true,
            IsActive = true,
            CreatedAtUtc = ts
        });

        b.Entity<ClientGrantType>().HasData(
            new ClientGrantType { Id = SeedConstants.GrantCodeId, ClientId = SeedConstants.ClientId, GrantType = "authorization_code" },
            new ClientGrantType { Id = SeedConstants.GrantRefreshId, ClientId = SeedConstants.ClientId, GrantType = "refresh_token" });

        b.Entity<ClientRedirectUri>().HasData(new ClientRedirectUri
        {
            Id = SeedConstants.RedirectUriId,
            ClientId = SeedConstants.ClientId,
            Uri = "http://localhost:5173/callback"
        });

        b.Entity<ClientCorsOrigin>().HasData(new ClientCorsOrigin
        {
            Id = SeedConstants.CorsOriginId,
            ClientId = SeedConstants.ClientId,
            Origin = "http://localhost:5173"
        });

        b.Entity<ApiResource>().HasData(new ApiResource
        {
            Id = SeedConstants.ApiResourceId,
            OrganizationId = SeedConstants.OrganizationId,
            ApplicationId = SeedConstants.ApplicationId,
            Name = "api-sentry-management",
            DisplayName = "Sentry Management API",
            IsActive = true,
            CreatedAtUtc = ts
        });

        // One scope per management area the Admin Management API already exposes (Organizations,
        // Applications, ApiResources+Scopes, Clients, Roles, Users+RoleAssignments, AuditLog) — a
        // real, complete administrative capability, not a symbolic pair (FR-024).
        b.Entity<Scope>().HasData(
            new Scope { Id = SeedConstants.ScopeOrganizationsManageId, OrganizationId = SeedConstants.OrganizationId, ApiResourceId = SeedConstants.ApiResourceId, Name = "organizations.manage", DisplayName = "Manage organizations", CreatedAtUtc = ts },
            new Scope { Id = SeedConstants.ScopeApplicationsManageId, OrganizationId = SeedConstants.OrganizationId, ApiResourceId = SeedConstants.ApiResourceId, Name = "applications.manage", DisplayName = "Manage applications", CreatedAtUtc = ts },
            new Scope { Id = SeedConstants.ScopeResourcesManageId, OrganizationId = SeedConstants.OrganizationId, ApiResourceId = SeedConstants.ApiResourceId, Name = "resources.manage", DisplayName = "Manage API resources and scopes", CreatedAtUtc = ts },
            new Scope { Id = SeedConstants.ScopeClientsManageId, OrganizationId = SeedConstants.OrganizationId, ApiResourceId = SeedConstants.ApiResourceId, Name = "clients.manage", DisplayName = "Manage OAuth clients", CreatedAtUtc = ts },
            new Scope { Id = SeedConstants.ScopeRolesManageId, OrganizationId = SeedConstants.OrganizationId, ApiResourceId = SeedConstants.ApiResourceId, Name = "roles.manage", DisplayName = "Manage roles", CreatedAtUtc = ts },
            new Scope { Id = SeedConstants.ScopeUsersManageId, OrganizationId = SeedConstants.OrganizationId, ApiResourceId = SeedConstants.ApiResourceId, Name = "users.manage", DisplayName = "Manage users and role assignments", CreatedAtUtc = ts },
            new Scope { Id = SeedConstants.ScopeAuditReadId, OrganizationId = SeedConstants.OrganizationId, ApiResourceId = SeedConstants.ApiResourceId, Name = "audit.read", DisplayName = "Read the audit log", CreatedAtUtc = ts });

        b.Entity<Role>().HasData(new Role
        {
            Id = SeedConstants.RoleId,
            OrganizationId = SeedConstants.OrganizationId,
            Name = "GlobalAdministrator",
            Description = "Full administrative access to the Sentry Management API.",
            Level = SeedConstants.ManagementRoleLevel,
            CreatedAtUtc = ts
        });

        var allScopeIds = new[]
        {
            SeedConstants.ScopeOrganizationsManageId,
            SeedConstants.ScopeApplicationsManageId,
            SeedConstants.ScopeResourcesManageId,
            SeedConstants.ScopeClientsManageId,
            SeedConstants.ScopeRolesManageId,
            SeedConstants.ScopeUsersManageId,
            SeedConstants.ScopeAuditReadId
        };

        b.Entity<RoleScope>().HasData(
            allScopeIds.Select(scopeId => new RoleScope { RoleId = SeedConstants.RoleId, ScopeId = scopeId }));

        b.Entity<ClientAllowedScope>().HasData(
            allScopeIds.Select(scopeId => new ClientAllowedScope { ClientId = SeedConstants.ClientId, ScopeId = scopeId }));

        b.Entity<RoleAssignment>().HasData(new RoleAssignment
        {
            Id = SeedConstants.RoleAssignmentId,
            UserId = SeedConstants.AdminUserId,
            RoleId = SeedConstants.RoleId,
            OrganizationId = SeedConstants.OrganizationId,
            AssignedAtUtc = ts,
            CreatedAtUtc = ts
        });
    }
}
