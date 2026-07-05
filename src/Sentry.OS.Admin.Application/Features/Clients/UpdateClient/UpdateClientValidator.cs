using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Clients.UpdateClient;

public class UpdateClientValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AccessTokenLifetimeSeconds).GreaterThan(0);
        RuleFor(x => x.IdentityTokenLifetimeSeconds).GreaterThan(0);
        RuleFor(x => x.RefreshTokenLifetimeSeconds).GreaterThan(0);
    }
}
