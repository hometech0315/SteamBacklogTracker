using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Refit;
using SteamBacklogTracker.Application.DTOs;
using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.Web.Services;

/// <summary>
/// High-level service for game-related operations with caching and error handling
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Get all games with caching
    /// </summary>
    Task<ServiceResult<IEnumerable<GameDto>>> GetAllGamesAsync(bool forceRefresh = false);

    /// <summary>
    /// Get specific game by ID with caching
    /// </summary>
    Task<ServiceResult<GameDto>> GetGameByIdAsync(int id, bool forceRefresh = false);

    /// <summary>
    /// Get games by platform with caching
    /// </summary>
    Task<ServiceResult<IEnumerable<GameDto>>> GetGamesByPlatformAsync(Platform platform, bool forceRefresh = false);

    /// <summary>
    /// Search games (not cached due to dynamic nature)
    /// </summary>
    Task<ServiceResult<IEnumerable<GameDto>>> SearchGamesAsync(string searchTerm);

    /// <summary>
    /// Create a new game
    /// </summary>
    Task<ServiceResult<GameDto>> CreateGameAsync(CreateGameDto createGameDto);

    /// <summary>
    /// Update an existing game
    /// </summary>
    Task<ServiceResult<GameDto>> UpdateGameAsync(int id, UpdateGameDto updateGameDto);

    /// <summary>
    /// Delete a game
    /// </summary>
    Task<ServiceResult<bool>> DeleteGameAsync(int id);

    /// <summary>
    /// Sync Steam library
    /// </summary>
    Task<ServiceResult<bool>> SyncSteamLibraryAsync(string steamUserId);

    /// <summary>
    /// Sync Epic Games library
    /// </summary>
    Task<ServiceResult<bool>> SyncEpicGamesLibraryAsync();

    /// <summary>
    /// Get dashboard statistics with caching
    /// </summary>
    Task<ServiceResult<DashboardStatsDto>> GetDashboardStatsAsync(bool forceRefresh = false);

    /// <summary>
    /// Clear all cached data
    /// </summary>
    void ClearCache();
}

public class GameService : IGameService
{
    private readonly ISteamTrackerApi _apiClient;
    private readonly IMemoryCache _cache;
    private readonly IApiErrorHandler _errorHandler;
    private readonly ILoadingStateService _loadingStateService;
    private readonly ApiConfiguration _config;
    private readonly ILogger<GameService> _logger;

    // Cache keys
    private const string CACHE_KEY_ALL_GAMES = "all_games";
    private const string CACHE_KEY_DASHBOARD_STATS = "dashboard_stats";
    private const string CACHE_KEY_GAME_PREFIX = "game_";
    private const string CACHE_KEY_PLATFORM_PREFIX = "platform_games_";

    public GameService(
        ISteamTrackerApi apiClient,
        IMemoryCache cache,
        IApiErrorHandler errorHandler,
        ILoadingStateService loadingStateService,
        IOptions<ApiConfiguration> config,
        ILogger<GameService> logger)
    {
        _apiClient = apiClient;
        _cache = cache;
        _errorHandler = errorHandler;
        _loadingStateService = loadingStateService;
        _config = config.Value;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<GameDto>>> GetAllGamesAsync(bool forceRefresh = false)
    {
        const string cacheKey = CACHE_KEY_ALL_GAMES;

        if (!forceRefresh && _cache.TryGetValue(cacheKey, out IEnumerable<GameDto>? cachedGames))
        {
            _logger.LogDebug("Retrieved games from cache");
            return ServiceResult<IEnumerable<GameDto>>.Success(cachedGames!);
        }

        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.LoadingGames, async () =>
        {
            try
            {
                var response = await _apiClient.GetGamesAsync();

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var games = response.Content;
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_config.CacheTtlMinutes.GamesList)
                    };
                    
                    _cache.Set(cacheKey, games, cacheOptions);
                    _logger.LogDebug("Cached {Count} games", games.Count());

                    return ServiceResult<IEnumerable<GameDto>>.Success(games);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<IEnumerable<GameDto>>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<IEnumerable<GameDto>>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<GameDto>> GetGameByIdAsync(int id, bool forceRefresh = false)
    {
        var cacheKey = $"{CACHE_KEY_GAME_PREFIX}{id}";

        if (!forceRefresh && _cache.TryGetValue(cacheKey, out GameDto? cachedGame))
        {
            _logger.LogDebug("Retrieved game {GameId} from cache", id);
            return ServiceResult<GameDto>.Success(cachedGame!);
        }

        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.LoadingGameDetails, async () =>
        {
            try
            {
                var response = await _apiClient.GetGameByIdAsync(id);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var game = response.Content;
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_config.CacheTtlMinutes.GameDetails)
                    };
                    
                    _cache.Set(cacheKey, game, cacheOptions);
                    _logger.LogDebug("Cached game {GameId}: {GameName}", id, game.Name);

                    return ServiceResult<GameDto>.Success(game);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<GameDto>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<GameDto>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<IEnumerable<GameDto>>> GetGamesByPlatformAsync(Platform platform, bool forceRefresh = false)
    {
        var cacheKey = $"{CACHE_KEY_PLATFORM_PREFIX}{platform}";

        if (!forceRefresh && _cache.TryGetValue(cacheKey, out IEnumerable<GameDto>? cachedGames))
        {
            _logger.LogDebug("Retrieved {Platform} games from cache", platform);
            return ServiceResult<IEnumerable<GameDto>>.Success(cachedGames!);
        }

        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.LoadingGames, async () =>
        {
            try
            {
                var response = await _apiClient.GetGamesByPlatformAsync(platform);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var games = response.Content;
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_config.CacheTtlMinutes.GamesList)
                    };
                    
                    _cache.Set(cacheKey, games, cacheOptions);
                    _logger.LogDebug("Cached {Count} {Platform} games", games.Count(), platform);

                    return ServiceResult<IEnumerable<GameDto>>.Success(games);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<IEnumerable<GameDto>>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<IEnumerable<GameDto>>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<IEnumerable<GameDto>>> SearchGamesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return ServiceResult<IEnumerable<GameDto>>.Success(Enumerable.Empty<GameDto>());
        }

        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.LoadingGames, async () =>
        {
            try
            {
                var response = await _apiClient.SearchGamesAsync(searchTerm.Trim());

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    return ServiceResult<IEnumerable<GameDto>>.Success(response.Content);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<IEnumerable<GameDto>>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<IEnumerable<GameDto>>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<GameDto>> CreateGameAsync(CreateGameDto createGameDto)
    {
        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.CreatingGame, async () =>
        {
            try
            {
                var response = await _apiClient.CreateGameAsync(createGameDto);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    // Invalidate caches
                    InvalidateGameCaches();
                    
                    return ServiceResult<GameDto>.Success(response.Content);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<GameDto>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<GameDto>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<GameDto>> UpdateGameAsync(int id, UpdateGameDto updateGameDto)
    {
        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.UpdatingGame, async () =>
        {
            try
            {
                var response = await _apiClient.UpdateGameAsync(id, updateGameDto);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    // Invalidate caches
                    InvalidateGameCaches();
                    _cache.Remove($"{CACHE_KEY_GAME_PREFIX}{id}");
                    
                    return ServiceResult<GameDto>.Success(response.Content);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<GameDto>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<GameDto>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<bool>> DeleteGameAsync(int id)
    {
        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.DeletingGame, async () =>
        {
            try
            {
                var response = await _apiClient.DeleteGameAsync(id);

                if (response.IsSuccessStatusCode)
                {
                    // Invalidate caches
                    InvalidateGameCaches();
                    _cache.Remove($"{CACHE_KEY_GAME_PREFIX}{id}");
                    
                    return ServiceResult<bool>.Success(true);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<bool>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<bool>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<bool>> SyncSteamLibraryAsync(string steamUserId)
    {
        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.SyncingSteamLibrary, async () =>
        {
            try
            {
                var response = await _apiClient.SyncSteamLibraryAsync(steamUserId);

                if (response.IsSuccessStatusCode)
                {
                    // Invalidate all caches after sync
                    ClearCache();
                    
                    return ServiceResult<bool>.Success(true);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<bool>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<bool>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<bool>> SyncEpicGamesLibraryAsync()
    {
        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.SyncingEpicLibrary, async () =>
        {
            try
            {
                var response = await _apiClient.SyncEpicGamesLibraryAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Invalidate all caches after sync
                    ClearCache();
                    
                    return ServiceResult<bool>.Success(true);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<bool>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<bool>.Failure(error.Message, error.Details);
            }
        });
    }

    public async Task<ServiceResult<DashboardStatsDto>> GetDashboardStatsAsync(bool forceRefresh = false)
    {
        const string cacheKey = CACHE_KEY_DASHBOARD_STATS;

        if (!forceRefresh && _cache.TryGetValue(cacheKey, out DashboardStatsDto? cachedStats))
        {
            _logger.LogDebug("Retrieved dashboard stats from cache");
            return ServiceResult<DashboardStatsDto>.Success(cachedStats!);
        }

        return await _loadingStateService.ExecuteWithLoadingAsync(LoadingOperations.LoadingDashboard, async () =>
        {
            try
            {
                var response = await _apiClient.GetDashboardStatsAsync();

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var stats = response.Content;
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_config.CacheTtlMinutes.DashboardStats)
                    };
                    
                    _cache.Set(cacheKey, stats, cacheOptions);
                    _logger.LogDebug("Cached dashboard stats");

                    return ServiceResult<DashboardStatsDto>.Success(stats);
                }

                var error = await _errorHandler.HandleApiErrorAsync(response);
                return ServiceResult<DashboardStatsDto>.Failure(error.Message, error.Details);
            }
            catch (Exception ex)
            {
                var error = _errorHandler.HandleException(ex);
                return ServiceResult<DashboardStatsDto>.Failure(error.Message, error.Details);
            }
        });
    }

    public void ClearCache()
    {
        _cache.Remove(CACHE_KEY_ALL_GAMES);
        _cache.Remove(CACHE_KEY_DASHBOARD_STATS);
        
        // Clear platform-specific caches
        foreach (var platform in Enum.GetValues<Platform>())
        {
            _cache.Remove($"{CACHE_KEY_PLATFORM_PREFIX}{platform}");
        }

        _logger.LogInformation("All caches cleared");
    }

    private void InvalidateGameCaches()
    {
        _cache.Remove(CACHE_KEY_ALL_GAMES);
        _cache.Remove(CACHE_KEY_DASHBOARD_STATS);
        
        // Clear platform-specific caches
        foreach (var platform in Enum.GetValues<Platform>())
        {
            _cache.Remove($"{CACHE_KEY_PLATFORM_PREFIX}{platform}");
        }
    }
}

/// <summary>
/// Generic result wrapper for service operations
/// </summary>
public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public string? ErrorDetails { get; private set; }

    private ServiceResult() { }

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static ServiceResult<T> Failure(string message, string? details = null)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ErrorMessage = message,
            ErrorDetails = details
        };
    }
}