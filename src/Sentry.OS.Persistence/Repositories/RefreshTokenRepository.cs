using Microsoft.EntityFrameworkCore;
using Sentry.OS.Domain.Tokens;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IRefreshTokenRepository"/>. Ignores the organization query
/// filter (the IdP has no ambient organization); refresh tokens are append-only and never updated
/// destructively — rotation/revocation mutate tracked entities returned by these queries.
/// </summary>
public class RefreshTokenRepository(IdentityDbContext dbContext) : IRefreshTokenRepository
{
    public Task<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        dbContext.RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

    public async Task<IReadOnlyList<RefreshToken>> GetForwardLineageAsync(Guid refreshTokenId, CancellationToken cancellationToken)
    {
        var lineage = new List<RefreshToken>();
        var currentId = (Guid?)refreshTokenId;

        while (currentId is not null)
        {
            var token = await dbContext.RefreshTokens
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == currentId, cancellationToken);

            if (token is null)
            {
                break;
            }

            lineage.Add(token);
            currentId = token.ReplacedByTokenId;
        }

        return lineage;
    }

    public void Add(RefreshToken refreshToken) => dbContext.RefreshTokens.Add(refreshToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
