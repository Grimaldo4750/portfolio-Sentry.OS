using Microsoft.EntityFrameworkCore;
using Sentry.OS.Domain.Users;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IUserTokenRepository"/> over the shared <see cref="UserToken"/> table.</summary>
public class UserTokenRepository(IdentityDbContext dbContext) : IUserTokenRepository
{
    public Task<UserToken?> FindActiveByUserAndPurposeAsync(Guid userId, UserTokenPurpose purpose, CancellationToken cancellationToken) =>
        dbContext.UserTokens
            .Where(t => t.UserId == userId && t.Purpose == purpose && t.ConsumedAtUtc == null)
            .OrderByDescending(t => t.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<UserToken?> FindByTokenHashAsync(string tokenHash, UserTokenPurpose purpose, CancellationToken cancellationToken) =>
        dbContext.UserTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.Purpose == purpose && t.ConsumedAtUtc == null, cancellationToken);

    public void Add(UserToken userToken) => dbContext.UserTokens.Add(userToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
