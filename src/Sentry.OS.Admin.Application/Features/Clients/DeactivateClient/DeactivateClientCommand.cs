using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.DeactivateClient;

public record DeactivateClientCommand(Guid ApplicationId, Guid Id) : IRequest<DeactivateClientResponse>, IAuditableRequest
{
    public string AuditAction => "Client.Deactivated";

    public string AuditTargetType => nameof(Client);
}
