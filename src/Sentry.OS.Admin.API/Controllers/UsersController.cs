using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Users.AssignRoleToUser;
using Sentry.OS.Admin.Application.Features.Users.CreateUser;
using Sentry.OS.Admin.Application.Features.Users.DeactivateUser;
using Sentry.OS.Admin.Application.Features.Users.Dtos;
using Sentry.OS.Admin.Application.Features.Users.GetUserById;
using Sentry.OS.Admin.Application.Features.Users.ListUserRoleAssignments;
using Sentry.OS.Admin.Application.Features.Users.ListUsers;
using Sentry.OS.Admin.Application.Features.Users.RemoveRoleFromUser;
using Sentry.OS.Admin.Application.Features.Users.UpdateUser;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Controllers;

/// <summary>Manages users within an organization and their role assignments.</summary>
[ApiController]
[Authorize]
[Route("api/organizations/{organizationId:guid}/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a user in the organization.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create(
        Guid organizationId, [FromBody] UserCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateUserCommand
        {
            OrganizationId = organizationId,
            Email = request.Email,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { organizationId, id = result.Id }, ApiResponse<UserDto>.Success(result));
    }

    /// <summary>Lists users in the organization.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> List(
        Guid organizationId, [FromQuery] ListUsersQuery query, CancellationToken cancellationToken)
    {
        query.OrganizationId = organizationId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<UserDto>>.Success(result));
    }

    /// <summary>Gets a user by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(organizationId, id), cancellationToken);
        return Ok(ApiResponse<UserDto>.Success(result));
    }

    /// <summary>Updates a user's profile.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(
        Guid organizationId, Guid id, [FromBody] UserUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateUserCommand
        {
            OrganizationId = organizationId,
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ProfilePictureUrl = request.ProfilePictureUrl
        }, cancellationToken);

        return Ok(ApiResponse<UserDto>.Success(result));
    }

    /// <summary>Deactivates a user.</summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Deactivate(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeactivateUserCommand(organizationId, id), cancellationToken);
        return Ok(ApiResponse<UserDto>.Success(result));
    }

    /// <summary>Lists a user's role assignments.</summary>
    [HttpGet("{id:guid}/roles")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleAssignmentDto>>>> ListRoles(
        Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListUserRoleAssignmentsQuery(organizationId, id), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RoleAssignmentDto>>.Success(result));
    }

    /// <summary>Assigns a role to a user.</summary>
    [HttpPost("{id:guid}/roles")]
    public async Task<ActionResult<ApiResponse<RoleAssignmentDto>>> AssignRole(
        Guid organizationId, Guid id, [FromBody] RoleAssignmentRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AssignRoleToUserCommand
        {
            OrganizationId = organizationId,
            UserId = id,
            RoleId = request.RoleId
        }, cancellationToken);

        return Ok(ApiResponse<RoleAssignmentDto>.Success(result));
    }

    /// <summary>Removes a role assignment from a user.</summary>
    [HttpDelete("{id:guid}/roles/{roleId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> RemoveRole(
        Guid organizationId, Guid id, Guid roleId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveRoleFromUserCommand(organizationId, id, roleId), cancellationToken);
        return Ok(ApiResponse.Success());
    }
}
