using MediatR;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.TwoFactor.RequestTwoFactorCode;

public class RequestTwoFactorCodeHandler(IAuthUserRepository users, IUserTokenRepository userTokens, IEmailSender emailSender, TimeProvider timeProvider)
    : IRequestHandler<RequestTwoFactorCodeCommand, RequestTwoFactorCodeResponse>
{
    public async Task<RequestTwoFactorCodeResponse> Handle(RequestTwoFactorCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await users.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null || user.IsDisabled || !user.TwoFactorEnabled)
        {
            return new RequestTwoFactorCodeResponse(false);
        }

        await TwoFactorCodeIssuer.IssueAndSendAsync(userTokens, emailSender, user, timeProvider.GetUtcNow(), cancellationToken);

        return new RequestTwoFactorCodeResponse(true);
    }
}
