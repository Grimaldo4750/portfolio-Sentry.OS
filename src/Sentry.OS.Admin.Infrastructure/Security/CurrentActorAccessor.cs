using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Persistence.Abstractions;

namespace Sentry.OS.Admin.Infrastructure.Security;

/// <summary>
/// Resolves the authenticated caller's identity and organization context from the validated bearer
/// token's claims. Backs both the Application layer's <see cref="ICurrentActor"/> and the shared
/// Persistence layer's <see cref="ICurrentOrganization"/> so EF Core's organization-isolation query
/// filters are driven by the same values used for authorization checks.
/// </summary>
public class CurrentActorAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentActor, ICurrentOrganization
{
    public const string OrganizationClaimType = "organization_id";
    public const string GlobalAdministratorClaimType = "global_administrator";
    public const string RoleLevelClaimType = "role_level";

    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    /// <summary>
    /// The organization the caller is currently acting within. A global administrator acts within
    /// whichever organization the request route addresses; every other caller is confined to the
    /// organization carried by their own token, regardless of what the route asks for.
    /// </summary>
    public Guid? OrganizationId
    {
        get
        {
            if (IsGlobalAdministrator &&
                httpContextAccessor.HttpContext?.Request.RouteValues.TryGetValue("organizationId", out var routeValue) is true &&
                Guid.TryParse(routeValue?.ToString(), out var routeOrganizationId))
            {
                return routeOrganizationId;
            }

            var value = User?.FindFirstValue(OrganizationClaimType);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public bool IsGlobalAdministrator =>
        bool.TryParse(User?.FindFirstValue(GlobalAdministratorClaimType), out var isGlobal) && isGlobal;

    public int? HighestRoleLevel
    {
        get
        {
            var levels = User?.FindAll(RoleLevelClaimType)
                .Select(c => int.TryParse(c.Value, out var level) ? level : (int?)null)
                .Where(level => level.HasValue)
                .Select(level => level!.Value)
                .ToList();

            return levels is { Count: > 0 } ? levels.Max() : null;
        }
    }
}
