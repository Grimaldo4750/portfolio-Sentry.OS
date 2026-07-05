using Sentry.OS.Domain.Clients;

namespace Sentry.OS.IdentityServer.Application.Common;

/// <summary>Result of validating an incoming request against the seeded client (FR-007).</summary>
public record ClientValidationResult(Client? Client, string? ErrorCode, string? ErrorDescription)
{
    public bool IsValid => Client is not null;

    public static ClientValidationResult Success(Client client) => new(client, null, null);

    public static ClientValidationResult Failure(string errorCode, string errorDescription) =>
        new(null, errorCode, errorDescription);
}
