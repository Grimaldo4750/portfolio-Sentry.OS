using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Scopes.UpdateScope;

public class UpdateScopeValidator : AbstractValidator<UpdateScopeCommand>
{
    public UpdateScopeValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
    }
}
