using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.Core.Interfaces;

public interface ISteamApiService
{
    Task<IEnumerable<Game>> GetOwnedGamesAsync(string steamUserId);
    Task<Game?> GetGameDetailsAsync(string steamAppId);
    Task<IEnumerable<Achievement>> GetGameAchievementsAsync(string steamAppId, string steamUserId);
    Task<Dictionary<string, double>> GetGlobalAchievementPercentagesAsync(string steamAppId);
    Task<bool> ValidateSteamUserAsync(string steamUserId);
}