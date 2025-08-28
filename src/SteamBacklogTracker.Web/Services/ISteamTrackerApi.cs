using Refit;
using SteamBacklogTracker.Application.DTOs;
using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.Web.Services;

/// <summary>
/// Refit interface defining all Steam Backlog Tracker API endpoints
/// </summary>
public interface ISteamTrackerApi
{
    // Games endpoints
    /// <summary>
    /// Get all games with optional filtering
    /// </summary>
    [Get("/api/games")]
    Task<ApiResponse<IEnumerable<GameDto>>> GetGamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get specific game details by ID
    /// </summary>
    [Get("/api/games/{id}")]
    Task<ApiResponse<GameDto>> GetGameByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get games by platform
    /// </summary>
    [Get("/api/games/platform/{platform}")]
    Task<ApiResponse<IEnumerable<GameDto>>> GetGamesByPlatformAsync(Platform platform, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search games by search term
    /// </summary>
    [Get("/api/games/search")]
    Task<ApiResponse<IEnumerable<GameDto>>> SearchGamesAsync([Query] string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new game
    /// </summary>
    [Post("/api/games")]
    Task<ApiResponse<GameDto>> CreateGameAsync([Body] CreateGameDto createGameDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing game
    /// </summary>
    [Put("/api/games/{id}")]
    Task<ApiResponse<GameDto>> UpdateGameAsync(int id, [Body] UpdateGameDto updateGameDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a game by ID
    /// </summary>
    [Delete("/api/games/{id}")]
    Task<ApiResponse<object>> DeleteGameAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sync Steam library for a user
    /// </summary>
    [Post("/api/games/sync/steam/{steamUserId}")]
    Task<ApiResponse<object>> SyncSteamLibraryAsync(string steamUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sync Epic Games library
    /// </summary>
    [Post("/api/games/sync/epic")]
    Task<ApiResponse<object>> SyncEpicGamesLibraryAsync(CancellationToken cancellationToken = default);

    // Dashboard endpoints
    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [Get("/api/dashboard/stats")]
    Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}