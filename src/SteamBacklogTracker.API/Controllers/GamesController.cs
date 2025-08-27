using Microsoft.AspNetCore.Mvc;
using SteamBacklogTracker.Application.DTOs;
using SteamBacklogTracker.Application.Services;
using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames()
    {
        var games = await _gameService.GetAllGamesAsync();
        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetGame(int id)
    {
        var game = await _gameService.GetGameByIdAsync(id);
        
        if (game == null)
            return NotFound();

        return Ok(game);
    }

    [HttpGet("platform/{platform}")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByPlatform(Platform platform)
    {
        var games = await _gameService.GetGamesByPlatformAsync(platform);
        return Ok(games);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<GameDto>>> SearchGames([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("Search term is required");

        var games = await _gameService.SearchGamesAsync(searchTerm);
        return Ok(games);
    }

    [HttpPost]
    public async Task<ActionResult<GameDto>> CreateGame([FromBody] CreateGameDto createGameDto)
    {
        var game = await _gameService.CreateGameAsync(createGameDto);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GameDto>> UpdateGame(int id, [FromBody] UpdateGameDto updateGameDto)
    {
        if (id != updateGameDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var game = await _gameService.UpdateGameAsync(updateGameDto);
            return Ok(game);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        await _gameService.DeleteGameAsync(id);
        return NoContent();
    }

    [HttpPost("sync/steam/{steamUserId}")]
    public async Task<IActionResult> SyncSteamLibrary(string steamUserId)
    {
        var success = await _gameService.SyncSteamLibraryAsync(steamUserId);
        
        if (success)
            return Ok(new { message = "Steam library synced successfully" });
        else
            return BadRequest(new { message = "Failed to sync Steam library" });
    }

    [HttpPost("sync/epic")]
    public async Task<IActionResult> SyncEpicGamesLibrary()
    {
        var success = await _gameService.SyncEpicGamesLibraryAsync();
        
        if (success)
            return Ok(new { message = "Epic Games library synced successfully" });
        else
            return BadRequest(new { message = "Failed to sync Epic Games library" });
    }
}