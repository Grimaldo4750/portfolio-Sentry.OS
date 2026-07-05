using Mapster;
using MediatR;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Common.Repositories;
using Sentry.OS.Domain.Clients;
using DomainApplication = Sentry.OS.Domain.Applications.Application;

namespace Sentry.OS.Admin.Application.Features.Clients.CreateClient;

public class CreateClientHandler(IClientRepository clients, IApplicationRepository applications, ICurrentActor currentActor)
    : IRequestHandler<CreateClientCommand, CreateClientResponse>
{
    public async Task<CreateClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var application = await applications.GetByIdIgnoringOrganizationAsync(request.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainApplication), request.ApplicationId);

        currentActor.EnsureOrganizationAccess(application.OrganizationId);

        var client = new Client
        {
            OrganizationId = application.OrganizationId,
            ApplicationId = request.ApplicationId,
            ClientId = Guid.NewGuid().ToString("N"),
            DisplayName = request.DisplayName,
            RequirePkce = request.RequirePkce,
            RequireClientSecret = request.RequireClientSecret,
            AccessTokenLifetimeSeconds = request.AccessTokenLifetimeSeconds,
            IdentityTokenLifetimeSeconds = request.IdentityTokenLifetimeSeconds,
            RefreshTokenLifetimeSeconds = request.RefreshTokenLifetimeSeconds,
            RefreshTokenRotationEnabled = request.RefreshTokenRotationEnabled,
            IsActive = true
        };

        clients.Add(client);
        await clients.SaveChangesAsync(cancellationToken);

        return client.Adapt<CreateClientResponse>();
    }
}
