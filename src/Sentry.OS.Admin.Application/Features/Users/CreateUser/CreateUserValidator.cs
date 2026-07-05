using FluentValidation;

namespace Sentry.OS.Admin.Application.Features.Users.CreateUser;

/// <summary>Syntax-only rules; email/username uniqueness (business rules) are enforced by the handler.</summary>
public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(256);
    }
}
