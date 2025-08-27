using System.Text.Json;
using SteamBacklogTracker.Core.Entities;
using SteamBacklogTracker.Core.Interfaces;

namespace SteamBacklogTracker.Infrastructure.Services;

public class EpicGamesService : IEpicGamesService
{
    private readonly string _epicGamesDataPath;

    public EpicGamesService()
    {
        _epicGamesDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Epic", "EpicGamesLauncher", "Data", "Manifests"
        );
    }

    public async Task<IEnumerable<Game>> GetOwnedGamesFromLocalFilesAsync()
    {
        var games = new List<Game>();

        try
        {
            if (!Directory.Exists(_epicGamesDataPath))
                return games;

            var manifestFiles = Directory.GetFiles(_epicGamesDataPath, "*.item");

            foreach (var manifestFile in manifestFiles)
            {
                try
                {
                    var jsonContent = await File.ReadAllTextAsync(manifestFile);
                    var manifest = JsonSerializer.Deserialize<EpicGameManifest>(jsonContent);

                    if (manifest != null && !string.IsNullOrEmpty(manifest.DisplayName))
                    {
                        var game = new Game
                        {
                            EpicGameId = manifest.CatalogItemId,
                            Name = manifest.DisplayName,
                            Developer = manifest.DeveloperName,
                            Platform = Platform.EpicGames,
                            IsOwned = true,
                            CompletionStatus = CompletionStatus.NotStarted,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            SteamAppId = string.Empty
                        };

                        if (!string.IsNullOrEmpty(manifest.InstallLocation) && Directory.Exists(manifest.InstallLocation))
                        {
                            var installInfo = new DirectoryInfo(manifest.InstallLocation);
                            game.LastPlayed = installInfo.LastWriteTime;
                        }

                        games.Add(game);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        catch
        {
            // Return empty list if we can't access Epic Games data
        }

        return games;
    }

    public async Task<Game?> GetGameDetailsAsync(string epicGameId)
    {
        try
        {
            if (!Directory.Exists(_epicGamesDataPath))
                return null;

            var manifestFiles = Directory.GetFiles(_epicGamesDataPath, "*.item");

            foreach (var manifestFile in manifestFiles)
            {
                try
                {
                    var jsonContent = await File.ReadAllTextAsync(manifestFile);
                    var manifest = JsonSerializer.Deserialize<EpicGameManifest>(jsonContent);

                    if (manifest?.CatalogItemId == epicGameId)
                    {
                        var game = new Game
                        {
                            EpicGameId = manifest.CatalogItemId,
                            Name = manifest.DisplayName,
                            Developer = manifest.DeveloperName,
                            Platform = Platform.EpicGames,
                            IsOwned = true,
                            CompletionStatus = CompletionStatus.NotStarted,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            SteamAppId = string.Empty
                        };

                        if (!string.IsNullOrEmpty(manifest.InstallLocation) && Directory.Exists(manifest.InstallLocation))
                        {
                            var installInfo = new DirectoryInfo(manifest.InstallLocation);
                            game.LastPlayed = installInfo.LastWriteTime;
                        }

                        return game;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task SyncLocalGameLibraryAsync()
    {
        // This would sync with the database through a repository
        // For now, it's a placeholder that would be implemented with proper dependency injection
        await Task.CompletedTask;
    }
}

public class EpicGameManifest
{
    public string CatalogItemId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DeveloperName { get; set; } = string.Empty;
    public string InstallLocation { get; set; } = string.Empty;
}