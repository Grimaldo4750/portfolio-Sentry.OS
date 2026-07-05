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
/// migration script. Provides a consistent slice of the whole structure:
/// Organization → Application → (Client, ApiResource → Scopes) and an organization-owned Role,
/// with an admin user assigned that role via a role assignment.
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
            Name = "Sentry",
            Slug = "sentry",
            DisplayName = "Sentry Platform",
            IsActive = true,
            CreatedAtUtc = ts
        });

        b.Entity<User>().HasData(new User
        {
            Id = SeedConstants.AdminUserId,
            Email = "admin@sentry.os",
            NormalizedEmail = "ADMIN@SENTRY.OS",
            EmailVerified = true,
            UserName = "admin",
            PasswordHash = SeedConstants.AdminPasswordHash,
            SecurityStamp = SeedConstants.AdminSecurityStamp,
            FirstName = "Sentry",
            LastName = "Administrator",
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
            Name = "Sentry Admin Portal",
            Slug = "admin-portal",
            Description = "Administrative portal for the Sentry.OS platform.",
            IsActive = true,
            CreatedAtUtc = ts
        });

        b.Entity<Client>().HasData(new Client
        {
            Id = SeedConstants.ClientId,
            OrganizationId = SeedConstants.OrganizationId,
            ApplicationId = SeedConstants.ApplicationId,
            ClientId = "sentry-admin-portal",
            DisplayName = "Sentry Admin Portal (SPA)",
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
            Name = "sentry-admin-api",
            DisplayName = "Sentry Admin API",
            IsActive = true,
            CreatedAtUtc = ts
        });

        b.Entity<Scope>().HasData(
            new Scope
            {
                Id = SeedConstants.ScopeReadId,
                OrganizationId = SeedConstants.OrganizationId,
                ApiResourceId = SeedConstants.ApiResourceId,
                Name = "admin.read",
                DisplayName = "Read administrative data",
                CreatedAtUtc = ts
            },
            new Scope
            {
                Id = SeedConstants.ScopeWriteId,
                OrganizationId = SeedConstants.OrganizationId,
                ApiResourceId = SeedConstants.ApiResourceId,
                Name = "admin.write",
                DisplayName = "Modify administrative data",
                CreatedAtUtc = ts
            });

        b.Entity<Role>().HasData(new Role
        {
            Id = SeedConstants.RoleId,
            OrganizationId = SeedConstants.OrganizationId,
            Name = "OrganizationAdmin",
            Description = "Full administrative access within the organization.",
            Level = SeedConstants.OrganizationAdminRoleLevel,
            CreatedAtUtc = ts
        });

        b.Entity<RoleScope>().HasData(
            new RoleScope { RoleId = SeedConstants.RoleId, ScopeId = SeedConstants.ScopeReadId },
            new RoleScope { RoleId = SeedConstants.RoleId, ScopeId = SeedConstants.ScopeWriteId });

        b.Entity<ClientAllowedScope>().HasData(
            new ClientAllowedScope { ClientId = SeedConstants.ClientId, ScopeId = SeedConstants.ScopeReadId },
            new ClientAllowedScope { ClientId = SeedConstants.ClientId, ScopeId = SeedConstants.ScopeWriteId });

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
