using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Organizations.CreateOrganization;
using Sentry.OS.Admin.Application.Features.Organizations.DeactivateOrganization;
using Sentry.OS.Admin.Application.Features.Organizations.Dtos;
using Sentry.OS.Admin.Application.Features.Organizations.GetOrganizationById;
using Sentry.OS.Admin.Application.Features.Organizations.ListOrganizations;
using Sentry.OS.Admin.Application.Features.Organizations.UpdateOrganization;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Controllers;

/// <summary>Manages platform organizations, the top-level isolation boundary.</summary>
[ApiController]
[Authorize(Policy = "GlobalAdministrator")]
[Route("api/organizations")]
public class OrganizationsController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a new organization.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Create(
        [FromBody] OrganizationCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateOrganizationCommand { Name = request.Name, Slug = request.Slug, DisplayName = request.DisplayName },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<OrganizationDto>.Success(result));
    }

    /// <summary>Lists organizations.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<OrganizationDto>>>> List(
        [FromQuery] ListOrganizationsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<OrganizationDto>>.Success(result));
    }

    /// <summary>Gets an organization by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrganizationByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<OrganizationDto>.Success(result));
    }

    /// <summary>Updates an organization's name and display name.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Update(
        Guid id, [FromBody] OrganizationUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateOrganizationCommand { Id = id, Name = request.Name, DisplayName = request.DisplayName },
            cancellationToken);

        return Ok(ApiResponse<OrganizationDto>.Success(result));
    }

    /// <summary>Deactivates an organization.</summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeactivateOrganizationCommand(id), cancellationToken);
        return Ok(ApiResponse<OrganizationDto>.Success(result));
    }
}
