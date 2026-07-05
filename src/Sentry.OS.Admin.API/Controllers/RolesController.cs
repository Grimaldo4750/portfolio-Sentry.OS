using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Roles.AttachScopeToRole;
using Sentry.OS.Admin.Application.Features.Roles.CreateRole;
using Sentry.OS.Admin.Application.Features.Roles.DeleteRole;
using Sentry.OS.Admin.Application.Features.Roles.DetachScopeFromRole;
using Sentry.OS.Admin.Application.Features.Roles.Dtos;
using Sentry.OS.Admin.Application.Features.Roles.GetRoleById;
using Sentry.OS.Admin.Application.Features.Roles.ListRoles;
using Sentry.OS.Admin.Application.Features.Roles.UpdateRole;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Controllers;

/// <summary>Manages roles within an organization and the scopes attached to them.</summary>
[ApiController]
[Authorize]
[Route("api/organizations/{organizationId:guid}/roles")]
public class RolesController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a role.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Create(
        Guid organizationId, [FromBody] RoleCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateRoleCommand
        {
            OrganizationId = organizationId,
            Name = request.Name,
            Description = request.Description,
            Level = request.Level
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { organizationId, id = result.Id }, ApiResponse<RoleDto>.Success(result));
    }

    /// <summary>Lists roles in the organization.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleDto>>>> List(
        Guid organizationId, [FromQuery] ListRolesQuery query, CancellationToken cancellationToken)
    {
        query.OrganizationId = organizationId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<RoleDto>>.Success(result));
    }

    /// <summary>Gets a role by id, including its attached scopes.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetById(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRoleByIdQuery(organizationId, id), cancellationToken);
        return Ok(ApiResponse<RoleDto>.Success(result));
    }

    /// <summary>Updates a role's name, description, and level.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Update(
        Guid organizationId, Guid id, [FromBody] RoleUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateRoleCommand
        {
            OrganizationId = organizationId,
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Level = request.Level
        }, cancellationToken);

        return Ok(ApiResponse<RoleDto>.Success(result));
    }

    /// <summary>Deletes a role. Rejected if it still has assignments or scopes.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteRoleCommand(organizationId, id), cancellationToken);
        return Ok(ApiResponse.Success());
    }

    /// <summary>Attaches a scope to the role.</summary>
    [HttpPost("{id:guid}/scopes")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> AttachScope(
        Guid organizationId, Guid id, [FromBody] RoleScopeRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AttachScopeToRoleCommand
        {
            OrganizationId = organizationId,
            RoleId = id,
            ScopeId = request.ScopeId
        }, cancellationToken);

        return Ok(ApiResponse<RoleDto>.Success(result));
    }

    /// <summary>Detaches a scope from the role.</summary>
    [HttpDelete("{id:guid}/scopes/{scopeId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> DetachScope(
        Guid organizationId, Guid id, Guid scopeId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DetachScopeFromRoleCommand(organizationId, id, scopeId), cancellationToken);
        return Ok(ApiResponse.Success());
    }
}
