namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>Exposes the IdP's currently-published public signing keys (FR-014).</summary>
public interface IJwksProvider
{
    IReadOnlyList<JsonWebKeyData> GetPublicKeys();
}
