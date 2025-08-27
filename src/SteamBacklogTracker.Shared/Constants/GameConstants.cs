namespace SteamBacklogTracker.Shared.Constants;

public static class GameConstants
{
    public const int DefaultGamePageSize = 20;
    public const int MaxGamePageSize = 100;
    public const string SteamImageBaseUrl = "https://steamcdn-a.akamaihd.net/steam/apps/";
    public const string DefaultGameImage = "/images/default-game-cover.jpg";
}

public static class PlatformColors
{
    public const string Steam = "#1e88e5";
    public const string EpicGames = "#313131";
    public const string Both = "#9c27b0";
}

public static class CompletionStatusColors
{
    public const string NotStarted = "#757575";
    public const string InProgress = "#2196f3";
    public const string Completed = "#4caf50";
    public const string Abandoned = "#f44336";
    public const string OnHold = "#ff9800";
}