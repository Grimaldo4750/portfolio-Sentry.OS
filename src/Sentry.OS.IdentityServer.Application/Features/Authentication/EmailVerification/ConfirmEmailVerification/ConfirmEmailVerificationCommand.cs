using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Authentication.EmailVerification.ConfirmEmailVerification;

/// <summary>Confirms a previously issued email-verification token and marks the email verified (FR-031).</summary>
public record ConfirmEmailVerificationCommand(string Token) : IRequest<ConfirmEmailVerificationResponse>;
