using Microsoft.EntityFrameworkCore;
using Sentry.OS.Domain.Tokens;
using Sentry.OS.Persistence;
using Sentry.OS.Persistence.Repositories;

namespace Sentry.OS.IdentityServer.Tests.Tokens;

/// <summary>
/// Verifies the forward-lineage mechanism <see cref="RefreshTokensHandler"/>-equivalent logic relies
/// on for reuse detection (FR-009): walking from any token in a rotation chain reaches every
/// descendant, and every token along that chain can be revoked in one pass.
/// </summary>
public class RefreshRotationTests
{
    private static IdentityDbContext CreateInMemoryContext(string databaseName) =>
        new(new DbContextOptionsBuilder<IdentityDbContext>().UseInMemoryDatabase(databaseName).Options,
            new DesignTimeCurrentOrganization());

    [Fact]
    public async Task GetForwardLineage_Walks_The_Entire_Rotation_Chain()
    {
        using var context = CreateInMemoryContext(nameof(GetForwardLineage_Walks_The_Entire_Rotation_Chain));
        var repository = new RefreshTokenRepository(context);

        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var tokenA = new RefreshToken { Id = Guid.NewGuid(), OrganizationId = organizationId, UserId = userId, ClientId = clientId, TokenHash = "hash-a", CreatedAtUtc = now, ExpiresAtUtc = now.AddDays(14) };
        var tokenB = new RefreshToken { Id = Guid.NewGuid(), OrganizationId = organizationId, UserId = userId, ClientId = clientId, TokenHash = "hash-b", CreatedAtUtc = now, ExpiresAtUtc = now.AddDays(14) };
        var tokenC = new RefreshToken { Id = Guid.NewGuid(), OrganizationId = organizationId, UserId = userId, ClientId = clientId, TokenHash = "hash-c", CreatedAtUtc = now, ExpiresAtUtc = now.AddDays(14) };

        tokenA.ConsumedAtUtc = now;
        tokenA.ReplacedByTokenId = tokenB.Id;
        tokenB.ConsumedAtUtc = now;
        tokenB.ReplacedByTokenId = tokenC.Id;

        repository.Add(tokenA);
        repository.Add(tokenB);
        repository.Add(tokenC);
        await repository.SaveChangesAsync(CancellationToken.None);

        var lineage = await repository.GetForwardLineageAsync(tokenA.Id, CancellationToken.None);

        Assert.Equal(3, lineage.Count);
        Assert.Equal([tokenA.Id, tokenB.Id, tokenC.Id], lineage.Select(t => t.Id));
    }

    [Fact]
    public async Task Revoking_The_Lineage_Marks_Every_Unrevoked_Token_In_The_Chain()
    {
        using var context = CreateInMemoryContext(nameof(Revoking_The_Lineage_Marks_Every_Unrevoked_Token_In_The_Chain));
        var repository = new RefreshTokenRepository(context);

        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var tokenA = new RefreshToken { Id = Guid.NewGuid(), OrganizationId = organizationId, UserId = userId, ClientId = clientId, TokenHash = "hash-a2", CreatedAtUtc = now, ExpiresAtUtc = now.AddDays(14) };
        var tokenB = new RefreshToken { Id = Guid.NewGuid(), OrganizationId = organizationId, UserId = userId, ClientId = clientId, TokenHash = "hash-b2", CreatedAtUtc = now, ExpiresAtUtc = now.AddDays(14) };

        tokenA.ConsumedAtUtc = now;
        tokenA.ReplacedByTokenId = tokenB.Id;

        repository.Add(tokenA);
        repository.Add(tokenB);
        await repository.SaveChangesAsync(CancellationToken.None);

        var lineage = await repository.GetForwardLineageAsync(tokenA.Id, CancellationToken.None);
        foreach (var token in lineage)
        {
            token.RevokedAtUtc = now;
            token.RevocationReason = RefreshTokenRevocationReason.ReuseDetected;
        }
        await repository.SaveChangesAsync(CancellationToken.None);

        var reloaded = await repository.GetForwardLineageAsync(tokenA.Id, CancellationToken.None);
        Assert.All(reloaded, t => Assert.Equal(RefreshTokenRevocationReason.ReuseDetected, t.RevocationReason));
        Assert.All(reloaded, t => Assert.NotNull(t.RevokedAtUtc));
    }
}
