using System.Net;

namespace Sentry.OS.IdentityServer.Pages;

/// <summary>Minimal server-rendered two-factor code entry page (FR-032).</summary>
public static class TwoFactorPage
{
    public static string Render(
        Guid userId, string clientId, string redirectUri, string scope, string codeChallenge, string codeChallengeMethod,
        string? state, string? nonce, string? message)
    {
        var messageHtml = message is null
            ? string.Empty
            : $"""<p>{WebUtility.HtmlEncode(message)}</p>""";

        var hiddenFields = $"""
            <input type="hidden" name="user_id" value="{userId}" />
            <input type="hidden" name="client_id" value="{WebUtility.HtmlEncode(clientId)}" />
            <input type="hidden" name="redirect_uri" value="{WebUtility.HtmlEncode(redirectUri)}" />
            <input type="hidden" name="scope" value="{WebUtility.HtmlEncode(scope)}" />
            <input type="hidden" name="code_challenge" value="{WebUtility.HtmlEncode(codeChallenge)}" />
            <input type="hidden" name="code_challenge_method" value="{WebUtility.HtmlEncode(codeChallengeMethod)}" />
            <input type="hidden" name="state" value="{WebUtility.HtmlEncode(state ?? string.Empty)}" />
            <input type="hidden" name="nonce" value="{WebUtility.HtmlEncode(nonce ?? string.Empty)}" />
            """;

        return $"""
        <!DOCTYPE html>
        <html lang="en">
        <head><meta charset="utf-8" /><title>Two-factor verification - Sentry.OS</title></head>
        <body style="font-family: sans-serif; max-width: 360px; margin: 80px auto;">
          <h1>Enter your verification code</h1>
          {messageHtml}
          <form method="post" action="/connect/login/two-factor">
            {hiddenFields}
            <label>Verification code<br /><input type="text" name="code" inputmode="numeric" required autofocus /></label><br /><br />
            <button type="submit">Verify</button>
          </form>
          <form method="post" action="/connect/login/two-factor/resend" style="margin-top: 1em;">
            {hiddenFields}
            <button type="submit">Resend code</button>
          </form>
        </body>
        </html>
        """;
    }
}
