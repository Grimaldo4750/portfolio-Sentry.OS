using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Clients.CreateClient;

public class CreateClientValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AccessTokenLifetimeSeconds).GreaterThan(0);
        RuleFor(x => x.IdentityTokenLifetimeSeconds).GreaterThan(0);
        RuleFor(x => x.RefreshTokenLifetimeSeconds).GreaterThan(0);
    }
}
