namespace Sentry.OS.Persistence.Conventions;

/// <summary>
/// Central definition of the SQL schemas used to organize the identity model. Entity
/// configurations reference these constants so table-to-schema mapping stays consistent.
/// </summary>
public static class SchemaConventions
{
    /// <summary>Core identity: organizations, users, memberships, applications, claims, profile.</summary>
    public const string Identity = "Identity";

    /// <summary>Authorization: clients, resources, scopes, roles, role assignments, refresh tokens.</summary>
    public const string Authz = "Auth";

    /// <summary>Audit trail.</summary>
    public const string Audit = "Audit";
}
