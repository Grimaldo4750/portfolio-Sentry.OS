using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.ApiResources.CreateApiResource;

/// <summary>Syntax-only rules; name uniqueness (a business rule) is enforced by the handler.</summary>
public class CreateApiResourceValidator : AbstractValidator<CreateApiResourceCommand>
{
    public CreateApiResourceValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
    }
}
