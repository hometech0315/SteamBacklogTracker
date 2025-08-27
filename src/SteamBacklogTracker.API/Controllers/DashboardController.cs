using Microsoft.AspNetCore.Mvc;
using SteamBacklogTracker.Application.DTOs;
using SteamBacklogTracker.Application.Services;

namespace SteamBacklogTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IGameService _gameService;

    public DashboardController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var stats = await _gameService.GetDashboardStatsAsync();
        return Ok(stats);
    }
}