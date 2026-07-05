using Microsoft.EntityFrameworkCore;
using Sentry.OS.Domain.Users;
using Sentry.OS.Persistence;
using Sentry.OS.Persistence.Repositories;

namespace Sentry.OS.IdentityServer.Tests.Security;

/// <summary>
/// Verifies the one-time-code lifecycle backing email verification (FR-031) and two-factor
/// authentication (FR-032): creation, "most recent unconsumed" lookup, consumption, and that an
/// expired-but-unconsumed token is still returned by hash lookup (expiry is the caller's business
/// rule to enforce, per the repository-segregation principle — repositories are pure CRUD).
/// </summary>
public class UserTokenLifecycleTests
{
    private static IdentityDbContext CreateInMemoryContext(string databaseName) =>
        new(new DbContextOptionsBuilder<IdentityDbContext>().UseInMemoryDatabase(databaseName).Options,
            new DesignTimeCurrentOrganization());

    [Fact]
    public async Task Create_Then_FindByHash_Returns_The_Same_Token()
    {
        using var context = CreateInMemoryContext(nameof(Create_Then_FindByHash_Returns_The_Same_Token));
        var repository = new UserTokenRepository(context);
        var userId = Guid.NewGuid();

        repository.Add(new UserToken
        {
            UserId = userId,
            Purpose = UserTokenPurpose.TwoFactor,
            TokenHash = "hash-abc",
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        });
        await repository.SaveChangesAsync(CancellationToken.None);

        var found = await repository.FindByTokenHashAsync("hash-abc", UserTokenPurpose.TwoFactor, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(userId, found!.UserId);
    }

    [Fact]
    public async Task FindActiveByUserAndPurpose_Returns_The_Most_Recently_Created_Unconsumed_Token()
    {
        using var context = CreateInMemoryContext(nameof(FindActiveByUserAndPurpose_Returns_The_Most_Recently_Created_Unconsumed_Token));
        var repository = new UserTokenRepository(context);
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        repository.Add(new UserToken { UserId = userId, Purpose = UserTokenPurpose.TwoFactor, TokenHash = "older", CreatedAtUtc = now.AddMinutes(-5), ExpiresAtUtc = now.AddMinutes(5) });
        repository.Add(new UserToken { UserId = userId, Purpose = UserTokenPurpose.TwoFactor, TokenHash = "newer", CreatedAtUtc = now, ExpiresAtUtc = now.AddMinutes(10) });
        await repository.SaveChangesAsync(CancellationToken.None);

        var active = await repository.FindActiveByUserAndPurposeAsync(userId, UserTokenPurpose.TwoFactor, CancellationToken.None);

        Assert.NotNull(active);
        Assert.Equal("newer", active!.TokenHash);
    }

    [Fact]
    public async Task Consumed_Token_Is_No_Longer_Found_By_Hash()
    {
        using var context = CreateInMemoryContext(nameof(Consumed_Token_Is_No_Longer_Found_By_Hash));
        var repository = new UserTokenRepository(context);
        var userId = Guid.NewGuid();

        var token = new UserToken
        {
            UserId = userId,
            Purpose = UserTokenPurpose.EmailVerification,
            TokenHash = "one-time-hash",
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddHours(24)
        };
        repository.Add(token);
        await repository.SaveChangesAsync(CancellationToken.None);

        token.ConsumedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(CancellationToken.None);

        var found = await repository.FindByTokenHashAsync("one-time-hash", UserTokenPurpose.EmailVerification, CancellationToken.None);

        Assert.Null(found);
    }

    [Fact]
    public async Task Expired_Unconsumed_Token_Is_Still_Returned_By_Hash_Lookup_For_The_Caller_To_Reject()
    {
        using var context = CreateInMemoryContext(nameof(Expired_Unconsumed_Token_Is_Still_Returned_By_Hash_Lookup_For_The_Caller_To_Reject));
        var repository = new UserTokenRepository(context);
        var userId = Guid.NewGuid();

        repository.Add(new UserToken
        {
            UserId = userId,
            Purpose = UserTokenPurpose.TwoFactor,
            TokenHash = "expired-hash",
            CreatedAtUtc = DateTime.UtcNow.AddHours(-1),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-50)
        });
        await repository.SaveChangesAsync(CancellationToken.None);

        var found = await repository.FindByTokenHashAsync("expired-hash", UserTokenPurpose.TwoFactor, CancellationToken.None);

        Assert.NotNull(found);
        Assert.True(found!.ExpiresAtUtc < DateTime.UtcNow);
    }
}
