using MediatR;
using Sentry.OS.IdentityServer.Application.Features.Authentication.EmailVerification.ConfirmEmailVerification;
using Sentry.OS.IdentityServer.Application.Features.Authentication.EmailVerification.RequestEmailVerification;

namespace Sentry.OS.IdentityServer.Endpoints;

/// <summary>Maps the email-verification request/confirm account endpoints (FR-031).</summary>
public static class EmailVerificationEndpoint
{
    public static IEndpointRouteBuilder MapEmailVerificationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/account/email-verification/send", async (SendEmailVerificationRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RequestEmailVerificationCommand(request.Email), cancellationToken);
            return Results.Ok(new { succeeded = result.Succeeded });
        })
        .WithName("RequestEmailVerification")
        .WithTags("Account")
        .WithSummary("Requests an email-verification link (never reveals whether the email is registered).");

        app.MapGet("/account/email-verification/confirm", async (string token, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new ConfirmEmailVerificationCommand(token), cancellationToken);
            return result.Succeeded
                ? Results.Ok(new { succeeded = true })
                : Results.BadRequest(new { succeeded = false, error = "invalid_token" });
        })
        .WithName("ConfirmEmailVerification")
        .WithTags("Account")
        .WithSummary("Confirms a previously issued email-verification token.");

        return app;
    }
}

public record SendEmailVerificationRequest(string Email);
