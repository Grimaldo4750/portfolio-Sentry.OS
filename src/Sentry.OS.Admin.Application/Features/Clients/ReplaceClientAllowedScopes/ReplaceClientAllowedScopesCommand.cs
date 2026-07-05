using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Clients;

namespace Sentry.OS.Admin.Application.Features.Clients.ReplaceClientAllowedScopes;

public class ReplaceClientAllowedScopesCommand : IRequest<ReplaceClientAllowedScopesResponse>, IAuditableRequest
{
    public Guid ApplicationId { get; set; }

    public Guid Id { get; set; }

    public List<Guid> ScopeIds { get; set; } = [];

    public string AuditAction => "Client.AllowedScopesReplaced";

    public string AuditTargetType => nameof(Client);
}
