namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>
/// Server-side, single-use, short-lived store for authorization codes (FR-008). The implementation
/// decides the backing store (in-memory cache, distributed cache, etc.).
/// </summary>
public interface IAuthorizationCodeStore
{
    /// <summary>Issues a new single-use code bound to <paramref name="data"/> and returns it.</summary>
    string Issue(AuthorizationCodeData data);

    /// <summary>
    /// Attempts to consume (retrieve and immediately invalidate) a code. Returns <see langword="false"/>
    /// for an unknown, expired, or already-consumed code.
    /// </summary>
    bool TryConsume(string code, out AuthorizationCodeData? data);
}
