using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Refit;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamBacklogTracker.Web.Services;

/// <summary>
/// Service responsible for configuring and managing HTTP clients for API communication
/// </summary>
public class ApiClientService
{
    private readonly ApiConfiguration _configuration;
    private readonly ILogger<ApiClientService> _logger;

    public ApiClientService(IOptions<ApiConfiguration> configuration, ILogger<ApiClientService> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;
    }

    /// <summary>
    /// Configure HTTP client services for dependency injection
    /// </summary>
    public static void ConfigureServices(IServiceCollection services, ApiConfiguration config)
    {
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            })
        };

        // Create Polly retry policy with exponential backoff
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode && msg.StatusCode != HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
                retryCount: config.RetryAttempts,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    // Log retry attempts - context doesn't have built-in logger access
                });

        // Create circuit breaker policy
        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: config.CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromSeconds(30));

        // Register HTTP client with Polly policies
        services.AddHttpClient("SteamTrackerApi", client =>
        {
            client.BaseAddress = new Uri(config.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("User-Agent", "SteamBacklogTracker.Web/1.0");
        })
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(circuitBreakerPolicy)
        .AddHttpMessageHandler<ApiLoggingHandler>();

        // Register Refit client
        services.AddRefitClient<ISteamTrackerApi>(refitSettings)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(config.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
                client.DefaultRequestHeaders.Add("User-Agent", "SteamBacklogTracker.Web/1.0");
            })
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);
    }
}

/// <summary>
/// HTTP message handler for request/response logging
/// </summary>
public class ApiLoggingHandler : DelegatingHandler
{
    private readonly ILogger<ApiLoggingHandler> _logger;

    public ApiLoggingHandler(ILogger<ApiLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        _logger.LogDebug("HTTP {Method} {Url} [{RequestId}]", 
            request.Method, request.RequestUri, requestId);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            
            stopwatch.Stop();
            
            _logger.LogDebug("HTTP {StatusCode} {Url} [{RequestId}] - {ElapsedMs}ms",
                response.StatusCode, request.RequestUri, requestId, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "HTTP Error {Url} [{RequestId}] - {ElapsedMs}ms",
                request.RequestUri, requestId, stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}