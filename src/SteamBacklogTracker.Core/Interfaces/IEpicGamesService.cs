using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.Core.Interfaces;

public interface IEpicGamesService
{
    Task<IEnumerable<Game>> GetOwnedGamesFromLocalFilesAsync();
    Task<Game?> GetGameDetailsAsync(string epicGameId);
    Task SyncLocalGameLibraryAsync();
}