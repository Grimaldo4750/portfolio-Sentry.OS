using MediatR;
using Microsoft.Extensions.Logging;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;
using Sentry.OS.IdentityServer.Application.Common.Security;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.SignIn;

/// <summary>
/// Verifies the submitted credentials (FR-003), enforces lockout/disabled/inactive-organization
/// rejection (FR-011, edge cases), and either issues a one-time authorization code directly or,
/// for a two-factor-enabled user, issues and emails a one-time code and defers issuance until
/// <c>VerifyTwoFactorCode</c> succeeds (FR-032).
/// </summary>
public class SignInHandler(
    IAuthUserRepository users,
    IAuthClientRepository clients,
    IUserTokenRepository userTokens,
    IAuthorizationCodeStore authorizationCodes,
    IEmailSender emailSender,
    PasswordHasher passwordHasher,
    TimeProvider timeProvider,
    ILogger<SignInHandler> logger)
    : IRequestHandler<SignInCommand, SignInResponse>
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public async Task<SignInResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var validation = await ClientAuthorizationValidator.ValidateAsync(
            clients, request.ClientId, request.RedirectUri, request.CodeChallenge, request.CodeChallengeMethod, cancellationToken);

        if (!validation.IsValid)
        {
            logger.LogWarning("Sign-in rejected: {ErrorCode} for client {ClientId}", validation.ErrorCode, request.ClientId);
            return Fail(validation.ErrorCode!, validation.ErrorDescription!);
        }

        var client = validation.Client!;
        var now = timeProvider.GetUtcNow();

        var user = await users.FindByNormalizedEmailAsync(request.Email.ToUpperInvariant(), cancellationToken);
        if (user is null || user.IsDisabled)
        {
            logger.LogWarning("Sign-in failed: unknown or disabled account for client {ClientId}", request.ClientId);
            return Fail("access_denied", "The email or password is incorrect.");
        }

        if (user.LockoutEnabled && user.LockoutEndUtc.HasValue && user.LockoutEndUtc.Value > now.UtcDateTime)
        {
            logger.LogWarning("Sign-in failed: account {UserId} is locked out", user.Id);
            return Fail("access_denied", "The email or password is incorrect.");
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.AccessFailedCount++;
            if (user.LockoutEnabled && user.AccessFailedCount >= MaxFailedAttempts)
            {
                user.LockoutEndUtc = now.UtcDateTime.Add(LockoutDuration);
                logger.LogWarning("Account {UserId} locked out after {Attempts} failed attempts", user.Id, user.AccessFailedCount);
            }

            await users.SaveChangesAsync(cancellationToken);
            logger.LogWarning("Sign-in failed: incorrect password for account {UserId}", user.Id);
            return Fail("access_denied", "The email or password is incorrect.");
        }

        var membership = await users.FindHomeMembershipAsync(user.Id, cancellationToken);
        if (membership is null || !membership.IsActive || !membership.Organization.IsActive)
        {
            logger.LogWarning("Sign-in failed: account {UserId} has no active organization membership", user.Id);
            return Fail("access_denied", "The email or password is incorrect.");
        }

        user.AccessFailedCount = 0;
        user.LastLoginAtUtc = now.UtcDateTime;
        await users.SaveChangesAsync(cancellationToken);

        if (user.TwoFactorEnabled)
        {
            await TwoFactorCodeIssuer.IssueAndSendAsync(userTokens, emailSender, user, now, cancellationToken);
            logger.LogInformation("Password verified for account {UserId}; two-factor code issued", user.Id);
            return new SignInResponse(false, null, null, true, user.Id, null);
        }

        var code = authorizationCodes.Issue(new AuthorizationCodeData(
            client.Id, user.Id, membership.OrganizationId, request.RequestedScopes,
            request.RedirectUri, request.CodeChallenge, request.CodeChallengeMethod, request.Nonce));

        logger.LogInformation("Sign-in succeeded for account {UserId} via client {ClientId}", user.Id, client.ClientId);
        return new SignInResponse(true, null, null, false, null, code);
    }

    private static SignInResponse Fail(string errorCode, string errorDescription) =>
        new(false, errorCode, errorDescription, false, null, null);
}
