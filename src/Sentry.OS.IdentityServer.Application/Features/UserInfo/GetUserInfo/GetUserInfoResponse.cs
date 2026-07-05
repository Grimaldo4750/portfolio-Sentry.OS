namespace Sentry.OS.IdentityServer.Application.Features.UserInfo.GetUserInfo;

public record GetUserInfoResponse(
    string Sub,
    string? Name,
    string Email,
    bool EmailVerified,
    IReadOnlyDictionary<string, string> Claims);
