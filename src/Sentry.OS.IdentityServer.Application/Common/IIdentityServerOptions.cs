namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>The small subset of IdP configuration handlers need, without depending on a configuration framework.</summary>
public interface IIdentityServerOptions
{
    /// <summary>The IdP's own issuer/base URL (e.g. <c>https://localhost:5001</c>).</summary>
    string Issuer { get; }

    /// <summary>The default access-token audience — the seeded API resource's machine name (<c>api-sentry-management</c>).</summary>
    string DefaultAudience { get; }
}
