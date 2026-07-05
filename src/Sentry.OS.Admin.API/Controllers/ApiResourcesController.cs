using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.ApiResources.CreateApiResource;
using Sentry.OS.Admin.Application.Features.ApiResources.DeleteApiResource;
using Sentry.OS.Admin.Application.Features.ApiResources.Dtos;
using Sentry.OS.Admin.Application.Features.ApiResources.GetApiResourceById;
using Sentry.OS.Admin.Application.Features.ApiResources.ListApiResources;
using Sentry.OS.Admin.Application.Features.ApiResources.UpdateApiResource;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Controllers;

/// <summary>Manages API resources belonging to an application.</summary>
[ApiController]
[Authorize]
[Route("api/applications/{applicationId:guid}/resources")]
public class ApiResourcesController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates an API resource.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApiResourceDto>>> Create(
        Guid applicationId, [FromBody] ApiResourceCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateApiResourceCommand
        {
            ApplicationId = applicationId,
            Name = request.Name,
            DisplayName = request.DisplayName
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { applicationId, id = result.Id }, ApiResponse<ApiResourceDto>.Success(result));
    }

    /// <summary>Lists API resources for the application.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ApiResourceDto>>>> List(
        Guid applicationId, [FromQuery] ListApiResourcesQuery query, CancellationToken cancellationToken)
    {
        query.ApplicationId = applicationId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ApiResourceDto>>.Success(result));
    }

    /// <summary>Gets an API resource by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ApiResourceDto>>> GetById(Guid applicationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetApiResourceByIdQuery(applicationId, id), cancellationToken);
        return Ok(ApiResponse<ApiResourceDto>.Success(result));
    }

    /// <summary>Updates an API resource.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ApiResourceDto>>> Update(
        Guid applicationId, Guid id, [FromBody] ApiResourceUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateApiResourceCommand
        {
            ApplicationId = applicationId,
            Id = id,
            DisplayName = request.DisplayName
        }, cancellationToken);

        return Ok(ApiResponse<ApiResourceDto>.Success(result));
    }

    /// <summary>Deletes an API resource. Rejected if it still has scopes.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid applicationId, Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteApiResourceCommand(applicationId, id), cancellationToken);
        return Ok(ApiResponse.Success());
    }
}
