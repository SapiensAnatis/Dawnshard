using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Contracts;
using Redis.OM;

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
    public async Task<IActionResult> GameList([FromQuery] int? questId)
    {
        IRedisCollection<RedisGame> query = this.connectionProvider
            .RedisCollection<RedisGame>()
            .Where(x => x.Visible);

        if (questId is not null)
            query = query.Where(x => x.QuestId == questId);

        return this.Ok(await query.ToListAsync());
    }
}
