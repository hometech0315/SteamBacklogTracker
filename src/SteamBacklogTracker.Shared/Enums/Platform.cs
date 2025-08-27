namespace SteamBacklogTracker.Shared.Models;

public enum Platform
{
    Steam = 1,
    EpicGames = 2,
    Both = 3
}

public enum CompletionStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Abandoned = 3,
    OnHold = 4
}