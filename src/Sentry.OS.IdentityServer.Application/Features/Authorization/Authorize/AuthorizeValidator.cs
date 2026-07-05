using FluentValidation;

namespace Sentry.OS.IdentityServer.Application.Features.Authorization.Authorize;

/// <summary>Syntax-only checks; client/redirect/PKCE business validation happens in <see cref="AuthorizeHandler"/>.</summary>
public class AuthorizeValidator : AbstractValidator<AuthorizeQuery>
{
    public AuthorizeValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.RedirectUri).NotEmpty();
        RuleFor(x => x.ResponseType).NotEmpty();
    }
}
