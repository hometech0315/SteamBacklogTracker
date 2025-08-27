using Microsoft.EntityFrameworkCore;
using SteamBacklogTracker.Core.Entities;
using SteamBacklogTracker.Core.Interfaces;
using SteamBacklogTracker.Infrastructure.Data;

namespace SteamBacklogTracker.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly SteamBacklogContext _context;

    public GameRepository(SteamBacklogContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        return await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Achievements)
            .Include(g => g.GameTags).ThenInclude(gt => gt.Tag)
            .ToListAsync();
    }

    public async Task<Game?> GetGameByIdAsync(int id)
    {
        return await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Achievements)
            .Include(g => g.GameTags).ThenInclude(gt => gt.Tag)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Game?> GetGameBySteamAppIdAsync(string steamAppId)
    {
        return await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Achievements)
            .Include(g => g.GameTags).ThenInclude(gt => gt.Tag)
            .FirstOrDefaultAsync(g => g.SteamAppId == steamAppId);
    }

    public async Task<IEnumerable<Game>> GetGamesByPlatformAsync(Platform platform)
    {
        return await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Achievements)
            .Include(g => g.GameTags).ThenInclude(gt => gt.Tag)
            .Where(g => g.Platform == platform || g.Platform == Platform.Both)
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetRecentlyPlayedGamesAsync(int count = 10)
    {
        return await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Achievements)
            .Include(g => g.GameTags).ThenInclude(gt => gt.Tag)
            .Where(g => g.LastPlayed.HasValue)
            .OrderByDescending(g => g.LastPlayed)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm)
    {
        return await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Achievements)
            .Include(g => g.GameTags).ThenInclude(gt => gt.Tag)
            .Where(g => g.Name.Contains(searchTerm) || 
                       g.Developer!.Contains(searchTerm) || 
                       g.Publisher!.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<Game> AddGameAsync(Game game)
    {
        game.CreatedAt = DateTime.UtcNow;
        game.UpdatedAt = DateTime.UtcNow;
        
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<Game> UpdateGameAsync(Game game)
    {
        game.UpdatedAt = DateTime.UtcNow;
        
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task DeleteGameAsync(int id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game != null)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalGamesCountAsync()
    {
        return await _context.Games.CountAsync();
    }

    public async Task<int> GetTotalPlaytimeMinutesAsync()
    {
        return await _context.Games.SumAsync(g => g.PlaytimeMinutes);
    }

    public async Task<double> GetAchievementCompletionPercentageAsync()
    {
        var totalAchievements = await _context.Achievements.CountAsync();
        if (totalAchievements == 0) return 0;

        var unlockedAchievements = await _context.Achievements
            .CountAsync(a => a.IsUnlocked);

        return (double)unlockedAchievements / totalAchievements * 100;
    }
}