using FluentValidation;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.EmailVerification.RequestEmailVerification;

public class RequestEmailVerificationValidator : AbstractValidator<RequestEmailVerificationCommand>
{
    public RequestEmailVerificationValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
