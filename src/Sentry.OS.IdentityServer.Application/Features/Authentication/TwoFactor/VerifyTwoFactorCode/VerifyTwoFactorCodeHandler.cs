using MediatR;
using Microsoft.Extensions.Logging;
using Sentry.OS.Domain.Users;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.TwoFactor.VerifyTwoFactorCode;

public class VerifyTwoFactorCodeHandler(
    IAuthUserRepository users,
    IAuthClientRepository clients,
    IUserTokenRepository userTokens,
    IAuthorizationCodeStore authorizationCodes,
    TimeProvider timeProvider,
    ILogger<VerifyTwoFactorCodeHandler> logger)
    : IRequestHandler<VerifyTwoFactorCodeCommand, VerifyTwoFactorCodeResponse>
{
    public async Task<VerifyTwoFactorCodeResponse> Handle(VerifyTwoFactorCodeCommand request, CancellationToken cancellationToken)
    {
        var validation = await ClientAuthorizationValidator.ValidateAsync(
            clients, request.ClientId, request.RedirectUri, request.CodeChallenge, request.CodeChallengeMethod, cancellationToken);

        if (!validation.IsValid)
        {
            return Fail(validation.ErrorCode!, validation.ErrorDescription!);
        }

        var client = validation.Client!;
        var now = timeProvider.GetUtcNow();

        var user = await users.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null || user.IsDisabled)
        {
            return Fail("access_denied", "The verification code is incorrect or has expired.");
        }

        var tokenHash = TokenHashing.Hash(request.Code);
        var token = await userTokens.FindByTokenHashAsync(tokenHash, UserTokenPurpose.TwoFactor, cancellationToken);
        if (token is null || token.UserId != user.Id || token.ExpiresAtUtc <= now.UtcDateTime)
        {
            logger.LogWarning("Two-factor verification failed for account {UserId}: incorrect or expired code", user.Id);
            return Fail("access_denied", "The verification code is incorrect or has expired.");
        }

        token.ConsumedAtUtc = now.UtcDateTime;
        await userTokens.SaveChangesAsync(cancellationToken);

        var membership = await users.FindHomeMembershipAsync(user.Id, cancellationToken);
        if (membership is null || !membership.IsActive || !membership.Organization.IsActive)
        {
            return Fail("access_denied", "The verification code is incorrect or has expired.");
        }

        var code = authorizationCodes.Issue(new AuthorizationCodeData(
            client.Id, user.Id, membership.OrganizationId, request.RequestedScopes,
            request.RedirectUri, request.CodeChallenge, request.CodeChallengeMethod, request.Nonce));

        logger.LogInformation("Two-factor verification succeeded for account {UserId}", user.Id);
        return new VerifyTwoFactorCodeResponse(true, null, null, code);
    }

    private static VerifyTwoFactorCodeResponse Fail(string errorCode, string errorDescription) =>
        new(false, errorCode, errorDescription, null);
}
