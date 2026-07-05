using MediatR;
using Sentry.OS.Domain.Users;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.EmailVerification.ConfirmEmailVerification;

public class ConfirmEmailVerificationHandler(IAuthUserRepository users, IUserTokenRepository userTokens, TimeProvider timeProvider)
    : IRequestHandler<ConfirmEmailVerificationCommand, ConfirmEmailVerificationResponse>
{
    public async Task<ConfirmEmailVerificationResponse> Handle(ConfirmEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var tokenHash = TokenHashing.Hash(request.Token);

        var token = await userTokens.FindByTokenHashAsync(tokenHash, UserTokenPurpose.EmailVerification, cancellationToken);
        if (token is null || token.ExpiresAtUtc <= now.UtcDateTime)
        {
            return new ConfirmEmailVerificationResponse(false);
        }

        var user = await users.FindByIdAsync(token.UserId, cancellationToken);
        if (user is null)
        {
            return new ConfirmEmailVerificationResponse(false);
        }

        token.ConsumedAtUtc = now.UtcDateTime;
        user.EmailVerified = true;

        await userTokens.SaveChangesAsync(cancellationToken);

        return new ConfirmEmailVerificationResponse(true);
    }
}
