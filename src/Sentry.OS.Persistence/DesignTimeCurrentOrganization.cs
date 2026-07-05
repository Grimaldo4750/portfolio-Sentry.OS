using Sentry.OS.Persistence.Abstractions;

namespace Sentry.OS.Persistence;

/// <summary>
/// Placeholder <see cref="ICurrentOrganization"/> used at design time and until runtime
/// organization resolution is implemented in a later feature. Reports no ambient organization.
/// </summary>
public sealed class DesignTimeCurrentOrganization : ICurrentOrganization
{
    /// <inheritdoc />
    public Guid? OrganizationId => null;
}
