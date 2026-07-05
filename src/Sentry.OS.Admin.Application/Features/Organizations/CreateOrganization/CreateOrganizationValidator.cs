using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Organizations.CreateOrganization;

/// <summary>Syntax-only rules; slug uniqueness (a business rule) is enforced by the handler.</summary>
public class CreateOrganizationValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
    }
}
