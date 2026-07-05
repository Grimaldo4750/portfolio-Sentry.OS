namespace Sentry.OS.Persistence.Abstractions;

/// <summary>
/// Ambient organization context used by the persistence layer's global query filters to enforce
/// organization isolation. Runtime resolution (from the authenticated principal / request) is
/// wired in a later feature; this feature only defines the abstraction so filters can be
/// expressed.
/// </summary>
/// <remarks>
/// Lives under <c>Abstractions</c> rather than an <c>Application</c> layer namespace to avoid a
/// name clash with the <see cref="Domain.Applications.Application"/> entity.
/// </remarks>
public interface ICurrentOrganization
{
    /// <summary>The active organization identifier, or <c>null</c> when none is in scope.</summary>
    Guid? OrganizationId { get; }
}
