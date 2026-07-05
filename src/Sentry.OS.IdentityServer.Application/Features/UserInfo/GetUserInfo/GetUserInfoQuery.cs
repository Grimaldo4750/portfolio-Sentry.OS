using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.UserInfo.GetUserInfo;

/// <summary>Projects standard profile claims for the signed-in user identified by a validated access token's <c>sub</c> (FR-016).</summary>
public record GetUserInfoQuery(Guid UserId) : IRequest<GetUserInfoResponse?>;
