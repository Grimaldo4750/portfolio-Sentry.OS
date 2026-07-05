using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.DeleteScope;

public record DeleteScopeCommand(Guid ApiResourceId, Guid Id) : IRequest<Unit>, IAuditableRequest
{
    public string AuditAction => "Scope.Deleted";

    public string AuditTargetType => nameof(Scope);
}
