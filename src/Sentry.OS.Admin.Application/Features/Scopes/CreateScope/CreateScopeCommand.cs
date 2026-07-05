using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Domain.Resources;

namespace Sentry.OS.Admin.Application.Features.Scopes.CreateScope;

public class CreateScopeCommand : IRequest<CreateScopeResponse>, IAuditableRequest
{
    public Guid ApiResourceId { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Description { get; set; }

    public string AuditAction => "Scope.Created";

    public string AuditTargetType => nameof(Scope);
}
