using FluentValidation;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.RefreshTokens;

public class RefreshTokensValidator : AbstractValidator<RefreshTokensCommand>
{
    public RefreshTokensValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
    }
}
