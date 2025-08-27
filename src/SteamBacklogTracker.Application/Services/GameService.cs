using SteamBacklogTracker.Application.DTOs;
using SteamBacklogTracker.Core.Entities;
using SteamBacklogTracker.Core.Interfaces;

namespace SteamBacklogTracker.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly ISteamApiService _steamApiService;
    private readonly IEpicGamesService _epicGamesService;

    public GameService(
        IGameRepository gameRepository,
        ISteamApiService steamApiService,
        IEpicGamesService epicGamesService)
    {
        _gameRepository = gameRepository;
        _steamApiService = steamApiService;
        _epicGamesService = epicGamesService;
    }

    public async Task<IEnumerable<GameDto>> GetAllGamesAsync()
    {
        var games = await _gameRepository.GetAllGamesAsync();
        return games.Select(MapToGameDto);
    }

    public async Task<GameDto?> GetGameByIdAsync(int id)
    {
        var game = await _gameRepository.GetGameByIdAsync(id);
        return game != null ? MapToGameDto(game) : null;
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPlatformAsync(Platform platform)
    {
        var games = await _gameRepository.GetGamesByPlatformAsync(platform);
        return games.Select(MapToGameDto);
    }

    public async Task<IEnumerable<GameDto>> SearchGamesAsync(string searchTerm)
    {
        var games = await _gameRepository.SearchGamesAsync(searchTerm);
        return games.Select(MapToGameDto);
    }

    public async Task<GameDto> CreateGameAsync(CreateGameDto createGameDto)
    {
        var game = new Game
        {
            SteamAppId = createGameDto.SteamAppId,
            EpicGameId = createGameDto.EpicGameId,
            Name = createGameDto.Name,
            Description = createGameDto.Description,
            Developer = createGameDto.Developer,
            Publisher = createGameDto.Publisher,
            ReleaseDate = createGameDto.ReleaseDate,
            HeaderImage = createGameDto.HeaderImage,
            CapsuleImage = createGameDto.CapsuleImage,
            Price = createGameDto.Price,
            PriceFormatted = createGameDto.PriceFormatted,
            Platform = createGameDto.Platform,
            IsOwned = true,
            CompletionStatus = CompletionStatus.NotStarted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdGame = await _gameRepository.AddGameAsync(game);
        return MapToGameDto(createdGame);
    }

    public async Task<GameDto> UpdateGameAsync(UpdateGameDto updateGameDto)
    {
        var game = await _gameRepository.GetGameByIdAsync(updateGameDto.Id);
        if (game == null)
            throw new ArgumentException($"Game with ID {updateGameDto.Id} not found");

        game.CompletionStatus = updateGameDto.CompletionStatus;
        game.PlaytimeMinutes = updateGameDto.PlaytimeMinutes;
        game.LastPlayed = updateGameDto.LastPlayed;
        game.UpdatedAt = DateTime.UtcNow;

        var updatedGame = await _gameRepository.UpdateGameAsync(game);
        return MapToGameDto(updatedGame);
    }

    public async Task DeleteGameAsync(int id)
    {
        await _gameRepository.DeleteGameAsync(id);
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalGames = await _gameRepository.GetTotalGamesCountAsync();
        var totalPlaytimeMinutes = await _gameRepository.GetTotalPlaytimeMinutesAsync();
        var achievementPercentage = await _gameRepository.GetAchievementCompletionPercentageAsync();
        var recentGames = await _gameRepository.GetRecentlyPlayedGamesAsync(5);

        var allGames = await _gameRepository.GetAllGamesAsync();
        var completedGames = allGames.Count(g => g.CompletionStatus == CompletionStatus.Completed);
        var inProgressGames = allGames.Count(g => g.CompletionStatus == CompletionStatus.InProgress);
        var notStartedGames = allGames.Count(g => g.CompletionStatus == CompletionStatus.NotStarted);

        var platformStats = allGames
            .GroupBy(g => g.Platform)
            .Select(g => new PlatformStatsDto(
                g.Key.ToString(),
                g.Count(),
                g.Sum(game => game.PlaytimeMinutes) / 60))
            .ToList();

        return new DashboardStatsDto(
            totalGames,
            totalPlaytimeMinutes / 60,
            achievementPercentage,
            completedGames,
            inProgressGames,
            notStartedGames,
            recentGames.Select(MapToGameDto),
            platformStats
        );
    }

    public async Task<bool> SyncSteamLibraryAsync(string steamUserId)
    {
        try
        {
            var steamGames = await _steamApiService.GetOwnedGamesAsync(steamUserId);
            
            foreach (var steamGame in steamGames)
            {
                var existingGame = await _gameRepository.GetGameBySteamAppIdAsync(steamGame.SteamAppId);
                if (existingGame == null)
                {
                    await _gameRepository.AddGameAsync(steamGame);
                }
                else
                {
                    existingGame.PlaytimeMinutes = steamGame.PlaytimeMinutes;
                    existingGame.LastPlayed = steamGame.LastPlayed;
                    existingGame.UpdatedAt = DateTime.UtcNow;
                    await _gameRepository.UpdateGameAsync(existingGame);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SyncEpicGamesLibraryAsync()
    {
        try
        {
            await _epicGamesService.SyncLocalGameLibraryAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static GameDto MapToGameDto(Game game)
    {
        var totalAchievements = game.Achievements.Count;
        var unlockedAchievements = game.Achievements.Count(a => a.IsUnlocked);
        var achievementPercentage = totalAchievements > 0 
            ? (double)unlockedAchievements / totalAchievements * 100 
            : 0;

        return new GameDto(
            game.Id,
            game.SteamAppId,
            game.EpicGameId,
            game.Name,
            game.Description,
            game.Developer,
            game.Publisher,
            game.ReleaseDate,
            game.HeaderImage,
            game.CapsuleImage,
            game.Price,
            game.PriceFormatted,
            game.PlaytimeMinutes,
            game.LastPlayed,
            game.IsOwned,
            game.Platform,
            game.CompletionStatus,
            game.Genres.Select(g => new GenreDto(g.Id, g.Name, g.Description)),
            game.Achievements.Select(a => new AchievementDto(
                a.Id,
                a.SteamAchievementId,
                a.Name,
                a.Description,
                a.IconUrl,
                a.IconGrayUrl,
                a.IsUnlocked,
                a.UnlockedAt,
                a.GlobalPercentage,
                a.IsHidden)),
            game.GameTags.Select(gt => gt.Tag.Name),
            achievementPercentage
        );
    }
}