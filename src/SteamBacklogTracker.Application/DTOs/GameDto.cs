using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.Application.DTOs;

public record GameDto(
    int Id,
    string SteamAppId,
    string? EpicGameId,
    string Name,
    string? Description,
    string? Developer,
    string? Publisher,
    DateTime? ReleaseDate,
    string? HeaderImage,
    string? CapsuleImage,
    decimal Price,
    string? PriceFormatted,
    int PlaytimeMinutes,
    DateTime? LastPlayed,
    bool IsOwned,
    Platform Platform,
    CompletionStatus CompletionStatus,
    IEnumerable<GenreDto> Genres,
    IEnumerable<AchievementDto> Achievements,
    IEnumerable<string> Tags,
    double AchievementCompletionPercentage
);

public record CreateGameDto(
    string SteamAppId,
    string? EpicGameId,
    string Name,
    string? Description,
    string? Developer,
    string? Publisher,
    DateTime? ReleaseDate,
    string? HeaderImage,
    string? CapsuleImage,
    decimal Price,
    string? PriceFormatted,
    Platform Platform
);

public record UpdateGameDto(
    int Id,
    CompletionStatus CompletionStatus,
    int PlaytimeMinutes,
    DateTime? LastPlayed
);