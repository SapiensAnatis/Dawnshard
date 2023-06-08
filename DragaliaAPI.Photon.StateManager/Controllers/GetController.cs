using System.Reflection;
using DragaliaAPI.Photon.Dto.Game;
using DragaliaAPI.Photon.StateManager.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NRediSearch;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Controller to export room state to callers.
/// </summary>
[Route("[controller]")]
[ApiController]
public class GetController : ControllerBase
{
    private readonly IRedisService redisService;

    public GetController(IRedisService redisService)
    {
        this.redisService = redisService;
    }

    /// <summary>
    /// Get a list of all open games.
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(IEnumerable<Game>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GameList()
    {
        string searchString = "*";

        return this.Ok(this.redisService.SearchGames(searchString));
    }

    private void CreateIndex() { }
}
