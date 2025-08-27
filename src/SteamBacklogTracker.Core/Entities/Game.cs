namespace SteamBacklogTracker.Core.Entities;

public class Game
{
    public int Id { get; set; }
    public string SteamAppId { get; set; } = string.Empty;
    public string? EpicGameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? HeaderImage { get; set; }
    public string? CapsuleImage { get; set; }
    public decimal Price { get; set; }
    public string? PriceFormatted { get; set; }
    public int PlaytimeMinutes { get; set; }
    public DateTime? LastPlayed { get; set; }
    public bool IsOwned { get; set; }
    public Platform Platform { get; set; }
    public CompletionStatus CompletionStatus { get; set; }
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    public ICollection<GameTag> GameTags { get; set; } = new List<GameTag>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}