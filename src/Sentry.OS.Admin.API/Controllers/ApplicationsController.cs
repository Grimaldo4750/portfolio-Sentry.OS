using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Applications.CreateApplication;
using Sentry.OS.Admin.Application.Features.Applications.DeactivateApplication;
using Sentry.OS.Admin.Application.Features.Applications.Dtos;
using Sentry.OS.Admin.Application.Features.Applications.GetApplicationById;
using Sentry.OS.Admin.Application.Features.Applications.ListApplications;
using Sentry.OS.Admin.Application.Features.Applications.UpdateApplication;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Controllers;

/// <summary>Manages applications within an organization.</summary>
[ApiController]
[Authorize]
[Route("api/organizations/{organizationId:guid}/applications")]
public class ApplicationsController(IMediator mediator) : ControllerBase
{
    /// <summary>Registers a new application.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> Create(
        Guid organizationId, [FromBody] ApplicationCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateApplicationCommand
        {
            OrganizationId = organizationId,
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { organizationId, id = result.Id }, ApiResponse<ApplicationDto>.Success(result));
    }

    /// <summary>Lists applications in the organization.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ApplicationDto>>>> List(
        Guid organizationId, [FromQuery] ListApplicationsQuery query, CancellationToken cancellationToken)
    {
        query.OrganizationId = organizationId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ApplicationDto>>.Success(result));
    }

    /// <summary>Gets an application by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> GetById(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetApplicationByIdQuery(organizationId, id), cancellationToken);
        return Ok(ApiResponse<ApplicationDto>.Success(result));
    }

    /// <summary>Updates an application.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> Update(
        Guid organizationId, Guid id, [FromBody] ApplicationUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateApplicationCommand
        {
            OrganizationId = organizationId,
            Id = id,
            Name = request.Name,
            Description = request.Description
        }, cancellationToken);

        return Ok(ApiResponse<ApplicationDto>.Success(result));
    }

    /// <summary>Deactivates an application.</summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> Deactivate(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeactivateApplicationCommand(organizationId, id), cancellationToken);
        return Ok(ApiResponse<ApplicationDto>.Success(result));
    }
}
