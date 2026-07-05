using Microsoft.Extensions.Logging;
using Sentry.OS.IdentityServer.Application.Common;

namespace Sentry.OS.IdentityServer.Infrastructure.Email;

/// <summary>
/// Development transport for <see cref="IEmailSender"/>: never contacts a real mail server. It writes
/// the recipient, subject, and body (including the verification link or one-time code) to the
/// structured log at <see cref="LogLevel.Information"/> with a <c>[DEV EMAIL]</c> marker, so email
/// verification and two-factor flows can be exercised end-to-end locally without SMTP infrastructure.
/// A production deployment swaps in a real (e.g. SMTP-backed) <see cref="IEmailSender"/> implementation
/// without changing any calling code.
/// </summary>
public class DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[DEV EMAIL] To: {To} | Subject: {Subject} | Body: {Body}",
            to, subject, body);

        return Task.CompletedTask;
    }
}
