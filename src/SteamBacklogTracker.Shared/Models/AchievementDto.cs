namespace SteamBacklogTracker.Shared.Models;

public record AchievementDto(
    int Id,
    string SteamAchievementId,
    string Name,
    string? Description,
    string? IconUrl,
    string? IconGrayUrl,
    bool IsUnlocked,
    DateTime? UnlockedAt,
    double GlobalPercentage,
    bool IsHidden
);