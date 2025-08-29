namespace SteamBacklogTracker.Web.Services;

/// <summary>
/// Configuration settings for API client
/// </summary>
public class ApiConfiguration
{
    public const string SectionName = "ApiConfiguration";

    public string BaseUrl { get; set; } = "https://localhost:7159";
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryAttempts { get; set; } = 3;
    public int CircuitBreakerThreshold { get; set; } = 5;
    public CacheTtlConfiguration CacheTtlMinutes { get; set; } = new();
}

public class CacheTtlConfiguration
{
    public int DashboardStats { get; set; } = 5;
    public int GamesList { get; set; } = 10;
    public int GameDetails { get; set; } = 30;
}