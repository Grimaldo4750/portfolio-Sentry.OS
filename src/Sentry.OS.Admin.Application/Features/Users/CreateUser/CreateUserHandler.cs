using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Organizations;
using Sentry.OS.Domain.Users;

namespace Sentry.OS.Admin.Application.Features.Users.CreateUser;

public class CreateUserHandler(
    IUserRepository users,
    IOrganizationRepository organizations,
    ICurrentActor currentActor,
    TimeProvider timeProvider)
    : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        currentActor.EnsureOrganizationAccess(request.OrganizationId);

        var organization = await organizations.GetByIdAsync(request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Organization), request.OrganizationId);

        var normalizedEmail = request.Email.ToUpperInvariant();
        if (await users.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException("A user with this email already exists.");
        }

        if (await users.UserNameExistsAsync(request.UserName, cancellationToken))
        {
            throw new ConflictException("A user with this user name already exists.");
        }

        var user = new User
        {
            Email = request.Email,
            NormalizedEmail = normalizedEmail,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            // Admin-provisioned accounts require the user to complete IdentityServer's password-setup flow.
            PasswordHash = string.Empty,
            SecurityStamp = Guid.NewGuid().ToString("N")
        };

        var membership = new OrganizationMembership
        {
            Organization = organization,
            User = user,
            IsHomeOrganization = true,
            JoinedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };

        users.Add(user, membership);
        await users.SaveChangesAsync(cancellationToken);

        return user.Adapt<CreateUserResponse>();
    }
}
