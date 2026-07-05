namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>Sends a single email. Implementations decide the transport (SMTP, a dev log line, etc.).</summary>
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken);
}
