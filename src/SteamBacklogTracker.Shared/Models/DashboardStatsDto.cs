namespace SteamBacklogTracker.Shared.Models;

public record DashboardStatsDto(
    int TotalGames,
    int TotalPlaytimeHours,
    double AchievementCompletionPercentage,
    int CompletedGames,
    int InProgressGames,
    int NotStartedGames,
    IEnumerable<GameDto> RecentlyPlayed,
    IEnumerable<PlatformStatsDto> PlatformStats
);

public record PlatformStatsDto(
    string Platform,
    int GameCount,
    int PlaytimeHours
);