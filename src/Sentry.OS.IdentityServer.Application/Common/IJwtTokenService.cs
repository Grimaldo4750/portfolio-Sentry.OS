namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>
/// Signs access and identity tokens, composing every protocol claim dynamically at issuance
/// (Principle VI — no protocol claim is ever read back from persisted storage).
/// </summary>
public interface IJwtTokenService
{
    /// <summary>Creates a signed access token audienced for the requested API resource.</summary>
    JwtIssuanceResult CreateAccessToken(
        Guid userId,
        string clientPublicId,
        Guid organizationId,
        bool isGlobalAdministrator,
        IReadOnlyList<string> roleNames,
        IReadOnlyList<int> administrativeRoleLevels,
        IReadOnlyList<string> grantedScopes,
        string audience,
        int lifetimeSeconds);

    /// <summary>Creates a signed identity token describing the authenticated user for the requesting client.</summary>
    JwtIssuanceResult CreateIdentityToken(
        Guid userId,
        string clientPublicId,
        string? name,
        string email,
        bool emailVerified,
        DateTimeOffset authTimeUtc,
        string? nonce,
        int lifetimeSeconds);
}
