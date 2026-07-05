using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Applications.UpdateApplication;

public class UpdateApplicationValidator : AbstractValidator<UpdateApplicationCommand>
{
    public UpdateApplicationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
