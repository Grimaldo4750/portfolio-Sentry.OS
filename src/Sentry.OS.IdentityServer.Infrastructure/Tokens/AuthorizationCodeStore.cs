using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Sentry.OS.IdentityServer.Application.Common;

namespace Sentry.OS.IdentityServer.Infrastructure.Tokens;

/// <summary>
/// Server-side, single-use, ~60-second-lived store for authorization codes (FR-008). Backed by
/// <see cref="IMemoryCache"/>: sufficient for this single-node development IdP; swappable for a
/// distributed cache later without touching callers, since they depend only on
/// <see cref="IAuthorizationCodeStore"/>.
/// </summary>
public class AuthorizationCodeStore(IMemoryCache cache) : IAuthorizationCodeStore
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(60);

    public string Issue(AuthorizationCodeData data)
    {
        var code = GenerateCode();
        cache.Set(CacheKey(code), data, Lifetime);
        return code;
    }

    public bool TryConsume(string code, out AuthorizationCodeData? data)
    {
        var key = CacheKey(code);
        if (cache.TryGetValue(key, out data))
        {
            cache.Remove(key);
            return true;
        }

        data = null;
        return false;
    }

    private static string CacheKey(string code) => $"idp:authcode:{code}";

    private static string GenerateCode() => Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
}
