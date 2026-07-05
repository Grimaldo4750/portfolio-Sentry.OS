namespace Sentry.OS.Admin.Application.Common;

/// <summary>The authenticated caller, resolved from the validated bearer token's claims.</summary>
public interface ICurrentActor
{
    /// <summary>Identifier of the authenticated user, or <c>null</c> when unauthenticated.</summary>
    Guid? UserId { get; }

    /// <summary>The organization the caller is currently acting within.</summary>
    Guid? OrganizationId { get; }

    /// <summary>Whether the caller holds the recognized global-administrator claim.</summary>
    bool IsGlobalAdministrator { get; }

    /// <summary>The highest <c>Level</c> among the caller's currently assigned administrative roles, if any.</summary>
    int? HighestRoleLevel { get; }
}
