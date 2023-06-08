using System.Reflection;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NRediSearch;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Controller to export room state to callers.
/// </summary>
[Route("[controller]")]
[ApiController]
public class GetController : ControllerBase
{
    private readonly IRedisConnectionProvider connectionProvider;

    public GetController(IRedisConnectionProvider connectionProvider)
    {
        this.connectionProvider = connectionProvider;
    }

    /// <summary>
    /// Get a list of all open games.
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(IEnumerable<RedisGame>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GameList()
    {
        return this.Ok(await this.connectionProvider.RedisCollection<RedisGame>().ToListAsync());
    }

    private void CreateIndex() { }
}
