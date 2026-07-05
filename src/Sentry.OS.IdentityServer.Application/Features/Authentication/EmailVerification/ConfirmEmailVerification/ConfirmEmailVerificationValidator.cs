using FluentValidation;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.EmailVerification.ConfirmEmailVerification;

public class ConfirmEmailVerificationValidator : AbstractValidator<ConfirmEmailVerificationCommand>
{
    public ConfirmEmailVerificationValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}
