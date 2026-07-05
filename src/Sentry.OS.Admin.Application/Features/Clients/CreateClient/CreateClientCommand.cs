using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.CreateClient;

public class CreateClientCommand : IRequest<CreateClientResponse>, IAuditableRequest
{
    public Guid ApplicationId { get; set; }

    public string DisplayName { get; set; } = null!;

    public bool RequirePkce { get; set; } = true;

    public bool RequireClientSecret { get; set; }

    public int AccessTokenLifetimeSeconds { get; set; } = 3600;

    public int IdentityTokenLifetimeSeconds { get; set; } = 300;

    public int RefreshTokenLifetimeSeconds { get; set; } = 1209600;

    public bool RefreshTokenRotationEnabled { get; set; } = true;

    public string AuditAction => "Client.Created";

    public string AuditTargetType => nameof(Client);
}
