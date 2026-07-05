using System.Net;

namespace Sentry.OS.IdentityServer.Pages;

/// <summary>Minimal server-rendered credential-entry page for the Authorization Code + PKCE flow.</summary>
public static class LoginPage
{
    public static string Render(
        string clientId, string redirectUri, string scope, string codeChallenge, string codeChallengeMethod,
        string? state, string? nonce, string? errorMessage)
    {
        var errorHtml = errorMessage is null
            ? string.Empty
            : $"""<p style="color:#b00020">{WebUtility.HtmlEncode(errorMessage)}</p>""";

        return $"""
        <!DOCTYPE html>
        <html lang="en">
        <head><meta charset="utf-8" /><title>Sign in - Sentry.OS</title></head>
        <body style="font-family: sans-serif; max-width: 360px; margin: 80px auto;">
          <h1>Sign in to Sentry.OS</h1>
          {errorHtml}
          <form method="post" action="/connect/login">
            <input type="hidden" name="client_id" value="{WebUtility.HtmlEncode(clientId)}" />
            <input type="hidden" name="redirect_uri" value="{WebUtility.HtmlEncode(redirectUri)}" />
            <input type="hidden" name="scope" value="{WebUtility.HtmlEncode(scope)}" />
            <input type="hidden" name="code_challenge" value="{WebUtility.HtmlEncode(codeChallenge)}" />
            <input type="hidden" name="code_challenge_method" value="{WebUtility.HtmlEncode(codeChallengeMethod)}" />
            <input type="hidden" name="state" value="{WebUtility.HtmlEncode(state ?? string.Empty)}" />
            <input type="hidden" name="nonce" value="{WebUtility.HtmlEncode(nonce ?? string.Empty)}" />
            <label>Email<br /><input type="email" name="email" required autofocus /></label><br /><br />
            <label>Password<br /><input type="password" name="password" required /></label><br /><br />
            <button type="submit">Sign in</button>
          </form>
        </body>
        </html>
        """;
    }
}
