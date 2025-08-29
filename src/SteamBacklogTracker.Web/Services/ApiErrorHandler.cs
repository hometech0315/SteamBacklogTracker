using Refit;
using System.Net;
using System.Text.Json;

namespace SteamBacklogTracker.Web.Services;

/// <summary>
/// Service for handling API errors and converting them to user-friendly messages
/// </summary>
public interface IApiErrorHandler
{
    /// <summary>
    /// Handle API response and extract error information
    /// </summary>
    Task<ApiErrorResult> HandleApiErrorAsync<T>(ApiResponse<T> response);

    /// <summary>
    /// Handle general exceptions from API calls
    /// </summary>
    ApiErrorResult HandleException(Exception exception);

    /// <summary>
    /// Get user-friendly error message from HTTP status code
    /// </summary>
    string GetUserFriendlyMessage(HttpStatusCode statusCode);
}

public class ApiErrorHandler : IApiErrorHandler
{
    private readonly ILogger<ApiErrorHandler> _logger;

    public ApiErrorHandler(ILogger<ApiErrorHandler> logger)
    {
        _logger = logger;
    }

    public async Task<ApiErrorResult> HandleApiErrorAsync<T>(ApiResponse<T> response)
    {
        var errorMessage = GetUserFriendlyMessage(response.StatusCode);
        var details = string.Empty;

        try
        {
            if (response.Error?.Content != null)
            {
                details = response.Error.Content;
                
                // Try to parse error details if it's JSON
                if (details.StartsWith("{") || details.StartsWith("["))
                {
                    var errorDoc = JsonDocument.Parse(details);
                    if (errorDoc.RootElement.TryGetProperty("title", out var titleElement))
                    {
                        errorMessage = titleElement.GetString() ?? errorMessage;
                    }
                    else if (errorDoc.RootElement.TryGetProperty("message", out var messageElement))
                    {
                        errorMessage = messageElement.GetString() ?? errorMessage;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse error response content");
        }

        _logger.LogError("API Error: {StatusCode} - {Message} - {Details}", 
            response.StatusCode, errorMessage, details);

        return await Task.FromResult(new ApiErrorResult
        {
            StatusCode = response.StatusCode,
            Message = errorMessage,
            Details = details,
            IsRetryable = IsRetryableError(response.StatusCode)
        });
    }

    public ApiErrorResult HandleException(Exception exception)
    {
        var message = exception switch
        {
            HttpRequestException httpEx => "Network error occurred. Please check your connection.",
            TaskCanceledException => "Request timed out. Please try again.",
            ApiException apiEx => GetUserFriendlyMessage(apiEx.StatusCode),
            _ => "An unexpected error occurred. Please try again."
        };

        _logger.LogError(exception, "API Exception: {Message}", message);

        return new ApiErrorResult
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = message,
            Details = exception.Message,
            IsRetryable = IsRetryableException(exception)
        };
    }

    public string GetUserFriendlyMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Invalid request. Please check your input.",
            HttpStatusCode.Unauthorized => "You are not authorized to perform this action.",
            HttpStatusCode.Forbidden => "Access denied. You don't have permission to access this resource.",
            HttpStatusCode.NotFound => "The requested resource was not found.",
            HttpStatusCode.Conflict => "A conflict occurred. The resource may have been modified.",
            HttpStatusCode.UnprocessableEntity => "The request contains invalid data.",
            HttpStatusCode.TooManyRequests => "Too many requests. Please wait and try again.",
            HttpStatusCode.InternalServerError => "Server error occurred. Please try again later.",
            HttpStatusCode.BadGateway => "Gateway error. The service may be temporarily unavailable.",
            HttpStatusCode.ServiceUnavailable => "Service is temporarily unavailable. Please try again later.",
            HttpStatusCode.GatewayTimeout => "Gateway timeout. The service is taking too long to respond.",
            _ => $"An error occurred (HTTP {(int)statusCode}). Please try again."
        };
    }

    private static bool IsRetryableError(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.RequestTimeout => true,
            HttpStatusCode.TooManyRequests => true,
            HttpStatusCode.InternalServerError => true,
            HttpStatusCode.BadGateway => true,
            HttpStatusCode.ServiceUnavailable => true,
            HttpStatusCode.GatewayTimeout => true,
            _ => false
        };
    }

    private static bool IsRetryableException(Exception exception)
    {
        return exception switch
        {
            HttpRequestException => true,
            TaskCanceledException => false, // Don't retry timeouts
            _ => false
        };
    }
}

/// <summary>
/// Result of API error handling
/// </summary>
public class ApiErrorResult
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public bool IsRetryable { get; set; }
}