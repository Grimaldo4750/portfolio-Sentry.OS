using MediatR;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.IdentityServer.Application.Features.Authorization.Authorize;

public class AuthorizeHandler(IAuthClientRepository clients) : IRequestHandler<AuthorizeQuery, AuthorizeResponse>
{
    public async Task<AuthorizeResponse> Handle(AuthorizeQuery request, CancellationToken cancellationToken)
    {
        var validation = await ClientAuthorizationValidator.ValidateAsync(
            clients, request.ClientId, request.RedirectUri, request.CodeChallenge, request.CodeChallengeMethod, cancellationToken);

        if (!validation.IsValid)
        {
            // "invalid_client" means the client id or redirect location itself couldn't be trusted;
            // any other failure (e.g. missing PKCE) still occurred against a known, registered redirect.
            var canRedirect = validation.ErrorCode != "invalid_client";
            return new AuthorizeResponse(false, validation.ErrorCode, validation.ErrorDescription, canRedirect);
        }

        if (!string.Equals(request.ResponseType, "code", StringComparison.Ordinal))
        {
            return new AuthorizeResponse(false, "unsupported_response_type", "Only the 'code' response type is supported.", true);
        }

        return new AuthorizeResponse(true, null, null, true);
    }
}
