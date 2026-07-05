using FluentValidation;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.TwoFactor.VerifyTwoFactorCode;

public class VerifyTwoFactorCodeValidator : AbstractValidator<VerifyTwoFactorCodeCommand>
{
    public VerifyTwoFactorCodeValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.RedirectUri).NotEmpty();
    }
}
