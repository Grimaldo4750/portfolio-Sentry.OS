using MediatR;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.IdentityServer.Application.Features.UserInfo.GetUserInfo;

public class GetUserInfoHandler(IAuthUserRepository users) : IRequestHandler<GetUserInfoQuery, GetUserInfoResponse?>
{
    public async Task<GetUserInfoResponse?> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await users.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var name = $"{user.FirstName} {user.LastName}".Trim();
        var persistentClaims = await users.GetPersistentClaimsAsync(user.Id, cancellationToken);

        return new GetUserInfoResponse(
            user.Id.ToString(),
            string.IsNullOrWhiteSpace(name) ? null : name,
            user.Email,
            user.EmailVerified,
            persistentClaims.ToDictionary(c => c.ClaimType, c => c.ClaimValue));
    }
}
