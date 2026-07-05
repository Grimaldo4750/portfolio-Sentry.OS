using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.TwoFactor.RequestTwoFactorCode;

/// <summary>Requests a fresh two-factor code be emailed, invalidating any previous unconsumed code (FR-032).</summary>
public record RequestTwoFactorCodeCommand(Guid UserId) : IRequest<RequestTwoFactorCodeResponse>;
