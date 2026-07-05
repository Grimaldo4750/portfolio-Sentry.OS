using Sentry.OS.IdentityServer.Application.Common.Repositories;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>
/// Issues and emails a fresh email-based two-factor code, invalidating any previously issued,
/// still-unconsumed code for the same user so only the newest one verifies (FR-032).
/// </summary>
public static class TwoFactorCodeIssuer
{
    public static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);

    public static async Task IssueAndSendAsync(
        IUserTokenRepository userTokens,
        IEmailSender emailSender,
        User user,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var existing = await userTokens.FindActiveByUserAndPurposeAsync(user.Id, UserTokenPurpose.TwoFactor, cancellationToken);
        if (existing is not null)
        {
            existing.ConsumedAtUtc = now.UtcDateTime;
        }

        var rawCode = OneTimeCodeGenerator.GenerateNumericCode();
        userTokens.Add(new UserToken
        {
            UserId = user.Id,
            Purpose = UserTokenPurpose.TwoFactor,
            TokenHash = TokenHashing.Hash(rawCode),
            CreatedAtUtc = now.UtcDateTime,
            ExpiresAtUtc = now.UtcDateTime.Add(CodeLifetime)
        });

        await userTokens.SaveChangesAsync(cancellationToken);
        await emailSender.SendAsync(
            user.Email,
            "Your Sentry.OS sign-in code",
            $"Your sign-in code is {rawCode}. It expires in 10 minutes.",
            cancellationToken);
    }
}
