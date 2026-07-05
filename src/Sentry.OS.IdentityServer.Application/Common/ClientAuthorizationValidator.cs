using Sentry.OS.IdentityServer.Application.Common.Repositories;

namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>
/// Shared client/redirect/PKCE validation used by the authorize, sign-in, two-factor, and
/// token-exchange handlers (FR-007) — factored out to avoid re-implementing the same rule four
/// times, not a bypass of "handlers own business validation" (every caller is itself a Handler).
/// An unrecognized client id and a recognized-but-inactive client return the identical
/// <c>invalid_client</c> error so the two cases are indistinguishable to the caller.
/// </summary>
public static class ClientAuthorizationValidator
{
    public static async Task<ClientValidationResult> ValidateAsync(
        IAuthClientRepository clients,
        string clientPublicId,
        string redirectUri,
        string? codeChallenge,
        string? codeChallengeMethod,
        CancellationToken cancellationToken)
    {
        var client = await clients.FindByClientIdAsync(clientPublicId, cancellationToken);
        if (client is null || !client.IsActive)
        {
            return ClientValidationResult.Failure("invalid_client", "The client is unknown or inactive.");
        }

        if (!client.RedirectUris.Any(r => r.Uri == redirectUri))
        {
            return ClientValidationResult.Failure("invalid_client", "The redirect location is not registered for this client.");
        }

        if (client.RequirePkce && (string.IsNullOrEmpty(codeChallenge) || !string.Equals(codeChallengeMethod, "S256", StringComparison.Ordinal)))
        {
            return ClientValidationResult.Failure("invalid_request", "A valid PKCE code_challenge with method S256 is required.");
        }

        return ClientValidationResult.Success(client);
    }
}
