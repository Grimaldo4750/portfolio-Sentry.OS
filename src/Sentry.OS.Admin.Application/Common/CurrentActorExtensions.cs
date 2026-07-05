namespace Sentry.OS.Admin.Application.Common;

public static class CurrentActorExtensions
{
    /// <summary>Rejects the request unless the caller is a global administrator or the route's organization matches their own.</summary>
    public static void EnsureOrganizationAccess(this ICurrentActor currentActor, Guid requestedOrganizationId)
    {
        if (currentActor.IsGlobalAdministrator)
        {
            return;
        }

        if (currentActor.OrganizationId != requestedOrganizationId)
        {
            throw new ForbiddenException("You do not have access to this organization.");
        }
    }
}
