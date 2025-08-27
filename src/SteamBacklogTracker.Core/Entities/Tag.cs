namespace SteamBacklogTracker.Core.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<GameTag> GameTags { get; set; } = new List<GameTag>();
}

public class GameTag
{
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}