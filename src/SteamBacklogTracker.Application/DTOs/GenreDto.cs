namespace SteamBacklogTracker.Application.DTOs;

public record GenreDto(
    int Id,
    string Name,
    string? Description
);