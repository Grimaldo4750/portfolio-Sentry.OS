namespace Sentry.OS.Persistence.Envelope;

/// <summary>The standard response envelope every Admin.API endpoint MUST return.</summary>
public class ApiResponse<T>
{
    public ResponseCode ResponseCode { get; init; }

    public string ResponseMessage { get; init; } = string.Empty;

    public T? Data { get; init; }

    public static ApiResponse<T> Success(T data, string message = "Operation completed successfully.") =>
        new() { ResponseCode = ResponseCode.Success, ResponseMessage = message, Data = data };

    public static ApiResponse<T> Failure(ResponseCode code, string message) =>
        new() { ResponseCode = code, ResponseMessage = message };
}

/// <summary>Non-generic helper for envelope responses that carry no payload.</summary>
public static class ApiResponse
{
    public static ApiResponse<object?> Success(string message = "Operation completed successfully.") =>
        new() { ResponseCode = ResponseCode.Success, ResponseMessage = message, Data = null };

    public static ApiResponse<object?> Failure(ResponseCode code, string message) =>
        new() { ResponseCode = code, ResponseMessage = message, Data = null };
}
