using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Sentry.OS.IdentityServer.Application.Features.UserInfo.GetUserInfo;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps <c>GET /connect/userinfo</c> (FR-016) — requires a valid bearer access token.</summary>
public static class UserInfoEndpoint
{
    public static IEndpointRouteBuilder MapUserInfoEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/connect/userinfo", async (ClaimsPrincipal user, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (!Guid.TryParse(sub, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await mediator.Send(new GetUserInfoQuery(userId), cancellationToken);
            if (result is null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new
            {
                sub = result.Sub,
                name = result.Name,
                email = result.Email,
                email_verified = result.EmailVerified,
                claims = result.Claims
            });
        })
        .WithName("UserInfo")
        .WithTags("OAuth2 / OIDC Protocol")
        .WithSummary("Returns the signed-in user's standard profile claims for a valid access token.")
        .RequireAuthorization();

        return app;
    }
}
