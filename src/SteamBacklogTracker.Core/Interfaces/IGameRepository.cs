using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.Core.Interfaces;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllGamesAsync();
    Task<Game?> GetGameByIdAsync(int id);
    Task<Game?> GetGameBySteamAppIdAsync(string steamAppId);
    Task<IEnumerable<Game>> GetGamesByPlatformAsync(Platform platform);
    Task<IEnumerable<Game>> GetRecentlyPlayedGamesAsync(int count = 10);
    Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm);
    Task<Game> AddGameAsync(Game game);
    Task<Game> UpdateGameAsync(Game game);
    Task DeleteGameAsync(int id);
    Task<int> GetTotalGamesCountAsync();
    Task<int> GetTotalPlaytimeMinutesAsync();
    Task<double> GetAchievementCompletionPercentageAsync();
}