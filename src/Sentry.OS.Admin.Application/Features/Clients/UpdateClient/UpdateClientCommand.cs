using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.UpdateClient;

public class UpdateClientCommand : IRequest<UpdateClientResponse>, IAuditableRequest
{
    public Guid ApplicationId { get; set; }

    public Guid Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public bool RequirePkce { get; set; }

    public bool RequireClientSecret { get; set; }

    public int AccessTokenLifetimeSeconds { get; set; }

    public int IdentityTokenLifetimeSeconds { get; set; }

    public int RefreshTokenLifetimeSeconds { get; set; }

    public bool RefreshTokenRotationEnabled { get; set; }

    public string AuditAction => "Client.Updated";

    public string AuditTargetType => nameof(Client);
}
