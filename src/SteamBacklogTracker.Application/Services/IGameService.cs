using SteamBacklogTracker.Application.DTOs;
using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.Application.Services;

public interface IGameService
{
    Task<IEnumerable<GameDto>> GetAllGamesAsync();
    Task<GameDto?> GetGameByIdAsync(int id);
    Task<IEnumerable<GameDto>> GetGamesByPlatformAsync(Platform platform);
    Task<IEnumerable<GameDto>> SearchGamesAsync(string searchTerm);
    Task<GameDto> CreateGameAsync(CreateGameDto createGameDto);
    Task<GameDto> UpdateGameAsync(UpdateGameDto updateGameDto);
    Task DeleteGameAsync(int id);
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<bool> SyncSteamLibraryAsync(string steamUserId);
    Task<bool> SyncEpicGamesLibraryAsync();
}