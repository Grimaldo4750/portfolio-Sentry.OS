using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Roles.CreateRole;

/// <summary>Syntax-only rules; name uniqueness and the role-level constraint (business rules) are enforced by the handler.</summary>
public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
