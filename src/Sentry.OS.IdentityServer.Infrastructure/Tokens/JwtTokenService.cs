using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Sentry.OS.IdentityServer.Application.Common;
using Sentry.OS.IdentityServer.Infrastructure.Keys;

namespace Sentry.OS.IdentityServer.Infrastructure.Tokens;

/// <summary>
/// Signs access and identity tokens, composing every protocol claim dynamically at issuance
/// (Principle VI — no protocol claim is ever read back from persisted storage). Access-token claims
/// mirror exactly what <c>Sentry.OS.Admin.API</c>'s <c>CurrentActorAccessor</c> reads: <c>sub</c>,
/// <c>organization_id</c>, <c>global_administrator</c>, <c>role_level</c>, <c>scope</c>, <c>role</c>.
/// </summary>
public class JwtTokenService(SigningKeyProvider signingKeyProvider, IIdentityServerOptions options, TimeProvider timeProvider)
    : IJwtTokenService
{
    public JwtIssuanceResult CreateAccessToken(
        Guid userId,
        string clientPublicId,
        Guid organizationId,
        bool isGlobalAdministrator,
        IReadOnlyList<string> roleNames,
        IReadOnlyList<int> administrativeRoleLevels,
        IReadOnlyList<string> grantedScopes,
        string audience,
        int lifetimeSeconds)
    {
        var now = timeProvider.GetUtcNow();
        var expires = now.AddSeconds(lifetimeSeconds);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("azp", clientPublicId),
            new("organization_id", organizationId.ToString()),
            new("global_administrator", isGlobalAdministrator ? "true" : "false")
        };

        claims.AddRange(roleNames.Select(role => new Claim("role", role)));
        claims.AddRange(administrativeRoleLevels.Select(level => new Claim("role_level", level.ToString())));

        if (grantedScopes.Count > 0)
        {
            claims.Add(new Claim("scope", string.Join(' ', grantedScopes)));
        }

        return Sign(claims, audience, now, expires);
    }

    public JwtIssuanceResult CreateIdentityToken(
        Guid userId,
        string clientPublicId,
        string? name,
        string email,
        bool emailVerified,
        DateTimeOffset authTimeUtc,
        string? nonce,
        int lifetimeSeconds)
    {
        var now = timeProvider.GetUtcNow();
        var expires = now.AddSeconds(lifetimeSeconds);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new("azp", clientPublicId),
            new(JwtRegisteredClaimNames.Email, email),
            new("email_verified", emailVerified ? "true" : "false"),
            new("auth_time", authTimeUtc.ToUnixTimeSeconds().ToString())
        };

        if (!string.IsNullOrEmpty(name))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, name));
        }

        if (!string.IsNullOrEmpty(nonce))
        {
            claims.Add(new Claim("nonce", nonce));
        }

        return Sign(claims, clientPublicId, now, expires);
    }

    private JwtIssuanceResult Sign(List<Claim> claims, string audience, DateTimeOffset notBefore, DateTimeOffset expires)
    {
        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: audience,
            claims: claims,
            notBefore: notBefore.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: signingKeyProvider.SigningCredentials);

        return new JwtIssuanceResult(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
