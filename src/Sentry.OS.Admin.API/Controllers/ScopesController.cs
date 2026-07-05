using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Scopes.CreateScope;
using Sentry.OS.Admin.Application.Features.Scopes.DeleteScope;
using Sentry.OS.Admin.Application.Features.Scopes.Dtos;
using Sentry.OS.Admin.Application.Features.Scopes.GetScopeById;
using Sentry.OS.Admin.Application.Features.Scopes.ListScopes;
using Sentry.OS.Admin.Application.Features.Scopes.UpdateScope;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Controllers;

/// <summary>Manages scopes belonging to an API resource.</summary>
[ApiController]
[Authorize]
[Route("api/resources/{apiResourceId:guid}/scopes")]
public class ScopesController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a scope.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> Create(
        Guid apiResourceId, [FromBody] ScopeCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateScopeCommand
        {
            ApiResourceId = apiResourceId,
            Name = request.Name,
            DisplayName = request.DisplayName,
            Description = request.Description
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { apiResourceId, id = result.Id }, ApiResponse<ScopeDto>.Success(result));
    }

    /// <summary>Lists scopes for the API resource.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ScopeDto>>>> List(
        Guid apiResourceId, [FromQuery] ListScopesQuery query, CancellationToken cancellationToken)
    {
        query.ApiResourceId = apiResourceId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ScopeDto>>.Success(result));
    }

    /// <summary>Gets a scope by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> GetById(Guid apiResourceId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetScopeByIdQuery(apiResourceId, id), cancellationToken);
        return Ok(ApiResponse<ScopeDto>.Success(result));
    }

    /// <summary>Updates a scope.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> Update(
        Guid apiResourceId, Guid id, [FromBody] ScopeUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateScopeCommand
        {
            ApiResourceId = apiResourceId,
            Id = id,
            DisplayName = request.DisplayName,
            Description = request.Description
        }, cancellationToken);

        return Ok(ApiResponse<ScopeDto>.Success(result));
    }

    /// <summary>Deletes a scope. Rejected if it is still in use by roles or clients.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid apiResourceId, Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteScopeCommand(apiResourceId, id), cancellationToken);
        return Ok(ApiResponse.Success());
    }
}
