namespace Sentry.OS.IdentityServer.Application.Common.Security;

/// <summary>
/// Computes the scopes an access token may carry: the intersection of what the user's roles grant,
/// what the client is allowed to request, and what was actually requested (Principle VI — a client
/// MUST NEVER receive a scope the user lacks, and a user MUST NEVER receive a scope the client isn't
/// allowed to request). Requested-but-ungranted scopes are silently dropped, never an error.
/// </summary>
public class ScopeIntersectionResolver
{
    public IReadOnlyList<string> Resolve(
        IReadOnlyCollection<string> userGrantedScopes,
        IReadOnlyCollection<string> clientAllowedScopes,
        IReadOnlyCollection<string> requestedScopes)
    {
        var userSet = new HashSet<string>(userGrantedScopes, StringComparer.Ordinal);
        var clientSet = new HashSet<string>(clientAllowedScopes, StringComparer.Ordinal);
        var requestedSet = requestedScopes.Count > 0
            ? new HashSet<string>(requestedScopes, StringComparer.Ordinal)
            : null;

        return userSet
            .Where(clientSet.Contains)
            .Where(scope => requestedSet is null || requestedSet.Contains(scope))
            .OrderBy(scope => scope, StringComparer.Ordinal)
            .ToList();
    }
}
