using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Applications.CreateApplication;

/// <summary>Syntax-only rules; slug uniqueness (a business rule) is enforced by the handler.</summary>
public class CreateApplicationValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
    }
}
