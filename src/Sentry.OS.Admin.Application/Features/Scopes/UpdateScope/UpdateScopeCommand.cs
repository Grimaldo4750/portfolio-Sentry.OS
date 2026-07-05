using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.UpdateScope;

public class UpdateScopeCommand : IRequest<UpdateScopeResponse>, IAuditableRequest
{
    public Guid ApiResourceId { get; set; }

    public Guid Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public string? Description { get; set; }

    public string AuditAction => "Scope.Updated";

    public string AuditTargetType => nameof(Scope);
}
