using System.Collections.Concurrent;

namespace SteamBacklogTracker.Web.Services;

/// <summary>
/// Service for managing loading states across the application
/// </summary>
public interface ILoadingStateService
{
    /// <summary>
    /// Check if a specific operation is currently loading
    /// </summary>
    bool IsLoading(string operationKey);

    /// <summary>
    /// Get all currently loading operations
    /// </summary>
    IEnumerable<string> GetLoadingOperations();

    /// <summary>
    /// Set loading state for an operation
    /// </summary>
    void SetLoading(string operationKey, bool isLoading);

    /// <summary>
    /// Execute an operation with automatic loading state management
    /// </summary>
    Task<T> ExecuteWithLoadingAsync<T>(string operationKey, Func<Task<T>> operation);

    /// <summary>
    /// Execute an operation with automatic loading state management (no return value)
    /// </summary>
    Task ExecuteWithLoadingAsync(string operationKey, Func<Task> operation);

    /// <summary>
    /// Event raised when loading state changes
    /// </summary>
    event EventHandler<LoadingStateChangedEventArgs>? LoadingStateChanged;
}

public class LoadingStateService : ILoadingStateService
{
    private readonly ConcurrentDictionary<string, bool> _loadingStates = new();
    private readonly ILogger<LoadingStateService> _logger;

    public LoadingStateService(ILogger<LoadingStateService> logger)
    {
        _logger = logger;
    }

    public event EventHandler<LoadingStateChangedEventArgs>? LoadingStateChanged;

    public bool IsLoading(string operationKey)
    {
        return _loadingStates.GetValueOrDefault(operationKey, false);
    }

    public IEnumerable<string> GetLoadingOperations()
    {
        return _loadingStates.Where(kvp => kvp.Value).Select(kvp => kvp.Key);
    }

    public void SetLoading(string operationKey, bool isLoading)
    {
        if (isLoading)
        {
            _loadingStates[operationKey] = true;
        }
        else
        {
            _loadingStates.TryRemove(operationKey, out _);
        }

        _logger.LogDebug("Loading state changed: {OperationKey} = {IsLoading}", operationKey, isLoading);
        
        LoadingStateChanged?.Invoke(this, new LoadingStateChangedEventArgs(operationKey, isLoading));
    }

    public async Task<T> ExecuteWithLoadingAsync<T>(string operationKey, Func<Task<T>> operation)
    {
        SetLoading(operationKey, true);
        
        try
        {
            var result = await operation();
            return result;
        }
        finally
        {
            SetLoading(operationKey, false);
        }
    }

    public async Task ExecuteWithLoadingAsync(string operationKey, Func<Task> operation)
    {
        SetLoading(operationKey, true);
        
        try
        {
            await operation();
        }
        finally
        {
            SetLoading(operationKey, false);
        }
    }
}

/// <summary>
/// Event arguments for loading state changes
/// </summary>
public class LoadingStateChangedEventArgs : EventArgs
{
    public string OperationKey { get; }
    public bool IsLoading { get; }

    public LoadingStateChangedEventArgs(string operationKey, bool isLoading)
    {
        OperationKey = operationKey;
        IsLoading = isLoading;
    }
}

/// <summary>
/// Common operation keys for loading states
/// </summary>
public static class LoadingOperations
{
    public const string LoadingGames = "loading_games";
    public const string LoadingDashboard = "loading_dashboard";
    public const string LoadingGameDetails = "loading_game_details";
    public const string SyncingSteamLibrary = "syncing_steam_library";
    public const string SyncingEpicLibrary = "syncing_epic_library";
    public const string CreatingGame = "creating_game";
    public const string UpdatingGame = "updating_game";
    public const string DeletingGame = "deleting_game";
}