using FluentValidation;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.ExchangeAuthorizationCode;

public class ExchangeAuthorizationCodeValidator : AbstractValidator<ExchangeAuthorizationCodeCommand>
{
    public ExchangeAuthorizationCodeValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.RedirectUri).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.CodeVerifier).NotEmpty();
    }
}
