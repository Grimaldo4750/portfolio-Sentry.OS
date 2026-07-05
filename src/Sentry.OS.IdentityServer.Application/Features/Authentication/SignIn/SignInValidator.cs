using FluentValidation;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.SignIn;

/// <summary>Syntax-only checks; credential/lockout/client business validation happens in <see cref="SignInHandler"/>.</summary>
public class SignInValidator : AbstractValidator<SignInCommand>
{
    public SignInValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.RedirectUri).NotEmpty();
    }
}
