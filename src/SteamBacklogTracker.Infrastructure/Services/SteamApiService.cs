using System.Text.Json;
using SteamBacklogTracker.Core.Entities;
using SteamBacklogTracker.Core.Interfaces;

namespace SteamBacklogTracker.Infrastructure.Services;

public class SteamApiService : ISteamApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _steamApiKey;

    public SteamApiService(HttpClient httpClient, string steamApiKey)
    {
        _httpClient = httpClient;
        _steamApiKey = steamApiKey;
    }

    public async Task<IEnumerable<Game>> GetOwnedGamesAsync(string steamUserId)
    {
        try
        {
            var url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={_steamApiKey}&steamid={steamUserId}&include_appinfo=true&include_played_free_games=true";
            
            var response = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<SteamOwnedGamesResponse>(response);

            var games = new List<Game>();
            if (result?.Response?.Games != null)
            {
                foreach (var steamGame in result.Response.Games)
                {
                    var game = new Game
                    {
                        SteamAppId = steamGame.AppId.ToString(),
                        Name = steamGame.Name,
                        PlaytimeMinutes = steamGame.PlaytimeForever,
                        LastPlayed = steamGame.RtimeLastPlayed > 0 
                            ? DateTimeOffset.FromUnixTimeSeconds(steamGame.RtimeLastPlayed).DateTime 
                            : null,
                        HeaderImage = $"https://steamcdn-a.akamaihd.net/steam/apps/{steamGame.AppId}/header.jpg",
                        CapsuleImage = $"https://steamcdn-a.akamaihd.net/steam/apps/{steamGame.AppId}/capsule_231x87.jpg",
                        Platform = Platform.Steam,
                        IsOwned = true,
                        CompletionStatus = CompletionStatus.NotStarted,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    games.Add(game);
                }
            }

            return games;
        }
        catch
        {
            return new List<Game>();
        }
    }

    public async Task<Game?> GetGameDetailsAsync(string steamAppId)
    {
        try
        {
            var url = $"https://store.steampowered.com/api/appdetails?appids={steamAppId}";
            
            var response = await _httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            
            if (doc.RootElement.TryGetProperty(steamAppId, out var appElement) &&
                appElement.TryGetProperty("success", out var success) &&
                success.GetBoolean() &&
                appElement.TryGetProperty("data", out var data))
            {
                var game = new Game
                {
                    SteamAppId = steamAppId,
                    Name = data.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
                    Description = data.TryGetProperty("detailed_description", out var desc) ? desc.GetString() : null,
                    Developer = data.TryGetProperty("developers", out var devs) && devs.GetArrayLength() > 0 
                        ? devs[0].GetString() : null,
                    Publisher = data.TryGetProperty("publishers", out var pubs) && pubs.GetArrayLength() > 0 
                        ? pubs[0].GetString() : null,
                    ReleaseDate = data.TryGetProperty("release_date", out var releaseData) &&
                                 releaseData.TryGetProperty("date", out var releaseDateStr) &&
                                 DateTime.TryParse(releaseDateStr.GetString(), out var parsedDate) 
                        ? parsedDate : null,
                    HeaderImage = data.TryGetProperty("header_image", out var header) ? header.GetString() : null,
                    Platform = Platform.Steam,
                    IsOwned = true,
                    CompletionStatus = CompletionStatus.NotStarted,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                if (data.TryGetProperty("price_overview", out var priceData))
                {
                    if (priceData.TryGetProperty("final", out var finalPrice))
                        game.Price = finalPrice.GetDecimal() / 100m;
                    if (priceData.TryGetProperty("final_formatted", out var priceFormatted))
                        game.PriceFormatted = priceFormatted.GetString();
                }

                return game;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<Achievement>> GetGameAchievementsAsync(string steamAppId, string steamUserId)
    {
        try
        {
            var url = $"https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid={steamAppId}&key={_steamApiKey}&steamid={steamUserId}";
            
            var response = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<SteamAchievementsResponse>(response);

            var achievements = new List<Achievement>();
            if (result?.Playerstats?.Achievements != null)
            {
                foreach (var steamAchievement in result.Playerstats.Achievements)
                {
                    var achievement = new Achievement
                    {
                        SteamAchievementId = steamAchievement.ApiName,
                        Name = steamAchievement.Name,
                        Description = steamAchievement.Description,
                        IsUnlocked = steamAchievement.Achieved == 1,
                        UnlockedAt = steamAchievement.UnlockTime > 0 
                            ? DateTimeOffset.FromUnixTimeSeconds(steamAchievement.UnlockTime).DateTime 
                            : null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    achievements.Add(achievement);
                }
            }

            return achievements;
        }
        catch
        {
            return new List<Achievement>();
        }
    }

    public async Task<Dictionary<string, double>> GetGlobalAchievementPercentagesAsync(string steamAppId)
    {
        try
        {
            var url = $"https://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/?gameid={steamAppId}";
            
            var response = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<SteamGlobalAchievementsResponse>(response);

            var percentages = new Dictionary<string, double>();
            if (result?.AchievementPercentages?.Achievements != null)
            {
                foreach (var achievement in result.AchievementPercentages.Achievements)
                {
                    percentages[achievement.Name] = achievement.Percent;
                }
            }

            return percentages;
        }
        catch
        {
            return new Dictionary<string, double>();
        }
    }

    public async Task<bool> ValidateSteamUserAsync(string steamUserId)
    {
        try
        {
            var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={_steamApiKey}&steamids={steamUserId}";
            
            var response = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<SteamPlayerSummaryResponse>(response);

            return result?.Response?.Players?.Count > 0;
        }
        catch
        {
            return false;
        }
    }
}

public class SteamOwnedGamesResponse
{
    public SteamOwnedGamesData? Response { get; set; }
}

public class SteamOwnedGamesData
{
    public List<SteamGame>? Games { get; set; }
}

public class SteamGame
{
    public int AppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PlaytimeForever { get; set; }
    public long RtimeLastPlayed { get; set; }
}

public class SteamAchievementsResponse
{
    public SteamPlayerStats? Playerstats { get; set; }
}

public class SteamPlayerStats
{
    public List<SteamAchievement>? Achievements { get; set; }
}

public class SteamAchievement
{
    public string ApiName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Achieved { get; set; }
    public long UnlockTime { get; set; }
}

public class SteamGlobalAchievementsResponse
{
    public SteamGlobalAchievements? AchievementPercentages { get; set; }
}

public class SteamGlobalAchievements
{
    public List<SteamGlobalAchievement>? Achievements { get; set; }
}

public class SteamGlobalAchievement
{
    public string Name { get; set; } = string.Empty;
    public double Percent { get; set; }
}

public class SteamPlayerSummaryResponse
{
    public SteamPlayerSummaryData? Response { get; set; }
}

public class SteamPlayerSummaryData
{
    public List<object>? Players { get; set; }
}