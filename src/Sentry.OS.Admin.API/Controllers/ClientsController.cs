using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Clients.CreateClient;
using Sentry.OS.Admin.Application.Features.Clients.DeactivateClient;
using Sentry.OS.Admin.Application.Features.Clients.Dtos;
using Sentry.OS.Admin.Application.Features.Clients.GetClientById;
using Sentry.OS.Admin.Application.Features.Clients.ListClients;
using Sentry.OS.Admin.Application.Features.Clients.ReplaceClientAllowedScopes;
using Sentry.OS.Admin.Application.Features.Clients.UpdateClient;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Controllers;

/// <summary>Manages OAuth clients belonging to an application.</summary>
[ApiController]
[Authorize]
[Route("api/applications/{applicationId:guid}/clients")]
public class ClientsController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a client.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ClientDto>>> Create(
        Guid applicationId, [FromBody] ClientCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateClientCommand
        {
            ApplicationId = applicationId,
            DisplayName = request.DisplayName,
            RequirePkce = request.RequirePkce,
            RequireClientSecret = request.RequireClientSecret,
            AccessTokenLifetimeSeconds = request.AccessTokenLifetimeSeconds,
            IdentityTokenLifetimeSeconds = request.IdentityTokenLifetimeSeconds,
            RefreshTokenLifetimeSeconds = request.RefreshTokenLifetimeSeconds,
            RefreshTokenRotationEnabled = request.RefreshTokenRotationEnabled
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { applicationId, id = result.Id }, ApiResponse<ClientDto>.Success(result));
    }

    /// <summary>Lists clients for the application.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ClientDto>>>> List(
        Guid applicationId, [FromQuery] ListClientsQuery query, CancellationToken cancellationToken)
    {
        query.ApplicationId = applicationId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ClientDto>>.Success(result));
    }

    /// <summary>Gets a client by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ClientDto>>> GetById(Guid applicationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientByIdQuery(applicationId, id), cancellationToken);
        return Ok(ApiResponse<ClientDto>.Success(result));
    }

    /// <summary>Updates client settings.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ClientDto>>> Update(
        Guid applicationId, Guid id, [FromBody] ClientUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateClientCommand
        {
            ApplicationId = applicationId,
            Id = id,
            DisplayName = request.DisplayName,
            RequirePkce = request.RequirePkce,
            RequireClientSecret = request.RequireClientSecret,
            AccessTokenLifetimeSeconds = request.AccessTokenLifetimeSeconds,
            IdentityTokenLifetimeSeconds = request.IdentityTokenLifetimeSeconds,
            RefreshTokenLifetimeSeconds = request.RefreshTokenLifetimeSeconds,
            RefreshTokenRotationEnabled = request.RefreshTokenRotationEnabled
        }, cancellationToken);

        return Ok(ApiResponse<ClientDto>.Success(result));
    }

    /// <summary>Deactivates a client.</summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<ApiResponse<ClientDto>>> Deactivate(Guid applicationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeactivateClientCommand(applicationId, id), cancellationToken);
        return Ok(ApiResponse<ClientDto>.Success(result));
    }

    /// <summary>Replaces the client's allowed-scope set.</summary>
    [HttpPut("{id:guid}/scopes")]
    public async Task<ActionResult<ApiResponse<ClientDto>>> ReplaceAllowedScopes(
        Guid applicationId, Guid id, [FromBody] ReplaceClientAllowedScopesRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ReplaceClientAllowedScopesCommand
        {
            ApplicationId = applicationId,
            Id = id,
            ScopeIds = request.ScopeIds
        }, cancellationToken);

        return Ok(ApiResponse<ClientDto>.Success(result));
    }
}
