using FluentValidation;

namespace Sentry.OS.IdentityServer.Application.Features.Tokens.RevokeToken;

public class RevokeTokenValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}
