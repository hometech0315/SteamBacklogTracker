namespace SteamBacklogTracker.Core.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<Game> Games { get; set; } = new List<Game>();
}