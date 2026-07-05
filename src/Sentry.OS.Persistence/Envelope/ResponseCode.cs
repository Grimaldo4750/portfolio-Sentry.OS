namespace Sentry.OS.Persistence.Envelope;

/// <summary>Strongly-typed response code carried by every <see cref="ApiResponse{T}"/>.</summary>
public enum ResponseCode
{
    Success,
    ValidationError,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    InternalServerError
}
