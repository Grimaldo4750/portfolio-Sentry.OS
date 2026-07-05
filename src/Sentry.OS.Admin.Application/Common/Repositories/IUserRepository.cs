using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Common.Repositories;

/// <summary>CRUD access to <see cref="User"/> and its organization membership. Carries no business rules.</summary>
public interface IUserRepository
{
    Task<User?> GetInOrganizationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken);

    Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken);

    Task<(IReadOnlyList<User> Items, int TotalCount)> ListInOrganizationAsync(
        Guid organizationId, int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Adds a new user together with its home-organization membership.</summary>
    void Add(User user, OrganizationMembership membership);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
