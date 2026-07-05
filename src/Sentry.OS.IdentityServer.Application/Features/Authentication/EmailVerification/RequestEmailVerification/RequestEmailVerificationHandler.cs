using MediatR;
using Sentry.OS.Domain.Users;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.EmailVerification.RequestEmailVerification;

public class RequestEmailVerificationHandler(
    IAuthUserRepository users,
    IUserTokenRepository userTokens,
    IEmailSender emailSender,
    IIdentityServerOptions options,
    TimeProvider timeProvider)
    : IRequestHandler<RequestEmailVerificationCommand, RequestEmailVerificationResponse>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);

    public async Task<RequestEmailVerificationResponse> Handle(RequestEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await users.FindByNormalizedEmailAsync(request.Email.ToUpperInvariant(), cancellationToken);

        // Never reveal whether the email is registered (same enumeration protection as sign-in).
        if (user is null || user.EmailVerified)
        {
            return new RequestEmailVerificationResponse(true);
        }

        var now = timeProvider.GetUtcNow();

        var existing = await userTokens.FindActiveByUserAndPurposeAsync(user.Id, UserTokenPurpose.EmailVerification, cancellationToken);
        if (existing is not null)
        {
            existing.ConsumedAtUtc = now.UtcDateTime;
        }

        var rawToken = OneTimeCodeGenerator.GenerateUrlSafeToken();
        userTokens.Add(new UserToken
        {
            UserId = user.Id,
            Purpose = UserTokenPurpose.EmailVerification,
            TokenHash = TokenHashing.Hash(rawToken),
            CreatedAtUtc = now.UtcDateTime,
            ExpiresAtUtc = now.UtcDateTime.Add(TokenLifetime)
        });

        await userTokens.SaveChangesAsync(cancellationToken);

        var link = $"{options.Issuer}/account/email-verification/confirm?token={Uri.EscapeDataString(rawToken)}";

        await emailSender.SendAsync(
            user.Email,
            "Verify your Sentry.OS email address",
            $"Confirm your email address by visiting: {link}",
            cancellationToken);

        return new RequestEmailVerificationResponse(true);
    }
}
