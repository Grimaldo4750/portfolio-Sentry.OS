using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Roles.UpdateRole;

/// <summary>Syntax-only rules; the role-level constraint (a business rule) is enforced by the handler.</summary>
public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
