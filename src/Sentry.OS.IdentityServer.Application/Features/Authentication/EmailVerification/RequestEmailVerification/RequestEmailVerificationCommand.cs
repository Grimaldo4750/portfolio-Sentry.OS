using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.EmailVerification.RequestEmailVerification;

/// <summary>Requests an email-verification link be sent (FR-031). Always reports success to avoid email enumeration.</summary>
public record RequestEmailVerificationCommand(string Email) : IRequest<RequestEmailVerificationResponse>;
