using Sentry.OS.IdentityServer.Application.Common;

namespace Sentry.OS.IdentityServer.Tests.Integration;

/// <summary>Test double capturing every "sent" email in memory (in send order) so tests can extract verification links/codes.</summary>
public class TestEmailSender : IEmailSender
{
    private readonly List<(string To, string Subject, string Body)> _sentEmails = [];
    private readonly Lock _lock = new();

    /// <summary>Every email sent so far, in the exact order <see cref="SendAsync"/> was called.</summary>
    public IReadOnlyList<(string To, string Subject, string Body)> SentEmails
    {
        get
        {
            lock (_lock)
            {
                return [.. _sentEmails];
            }
        }
    }

    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _sentEmails.Add((to, subject, body));
        }

        return Task.CompletedTask;
    }
}
