using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.ApiResources.UpdateApiResource;

public class UpdateApiResourceValidator : AbstractValidator<UpdateApiResourceCommand>
{
    public UpdateApiResourceValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
    }
}
