using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Scopes.CreateScope;

/// <summary>Syntax-only rules; name uniqueness (a business rule) is enforced by the handler.</summary>
public class CreateScopeValidator : AbstractValidator<CreateScopeCommand>
{
    public CreateScopeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
    }
}
