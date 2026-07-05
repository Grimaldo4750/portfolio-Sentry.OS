using Sentry.OS.IdentityServer.Application.Common.Security;

namespace Sentry.OS.IdentityServer.Tests.Tokens;

public class ScopeIntersectionResolverTests
{
    private readonly ScopeIntersectionResolver _resolver = new();

    [Fact]
    public void Resolve_Returns_Exact_Match_When_User_And_Client_Grant_The_Same_Scopes()
    {
        var result = _resolver.Resolve(
            userGrantedScopes: ["organizations.manage", "users.manage"],
            clientAllowedScopes: ["organizations.manage", "users.manage"],
            requestedScopes: ["organizations.manage", "users.manage"]);

        Assert.Equal(["organizations.manage", "users.manage"], result);
    }

    [Fact]
    public void Resolve_Downscopes_When_Client_Requests_More_Than_The_User_Or_Client_Allows()
    {
        var result = _resolver.Resolve(
            userGrantedScopes: ["organizations.manage"],
            clientAllowedScopes: ["organizations.manage", "users.manage"],
            requestedScopes: ["organizations.manage", "users.manage", "audit.read"]);

        Assert.Equal(["organizations.manage"], result);
    }

    [Fact]
    public void Resolve_Returns_Empty_When_User_And_Client_Scopes_Are_Disjoint()
    {
        var result = _resolver.Resolve(
            userGrantedScopes: ["organizations.manage"],
            clientAllowedScopes: ["users.manage"],
            requestedScopes: ["organizations.manage", "users.manage"]);

        Assert.Empty(result);
    }

    [Fact]
    public void Resolve_Never_Grants_A_Scope_The_Client_Is_Not_Allowed_To_Request()
    {
        var result = _resolver.Resolve(
            userGrantedScopes: ["organizations.manage", "users.manage"],
            clientAllowedScopes: ["organizations.manage"],
            requestedScopes: ["organizations.manage", "users.manage"]);

        Assert.DoesNotContain("users.manage", result);
    }
}
