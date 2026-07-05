using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Middleware;

/// <summary>Maps every exception surfaced by a request into the standard <see cref="ApiResponse{T}"/> envelope.</summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var message = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage));
            await WriteAsync(context, HttpStatusCode.BadRequest, ResponseCode.ValidationError, message);
        }
        catch (NotFoundException ex)
        {
            await WriteAsync(context, HttpStatusCode.NotFound, ResponseCode.NotFound, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteAsync(context, HttpStatusCode.Conflict, ResponseCode.Conflict, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await WriteAsync(context, HttpStatusCode.Forbidden, ResponseCode.Forbidden, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteAsync(context, HttpStatusCode.Unauthorized, ResponseCode.Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception processing {Path}", context.Request.Path);
            await WriteAsync(context, HttpStatusCode.InternalServerError, ResponseCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteAsync(HttpContext context, HttpStatusCode statusCode, ResponseCode code, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(ApiResponse.Failure(code, message));
    }
}
