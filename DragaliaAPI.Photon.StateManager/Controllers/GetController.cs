using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Contracts;
using Redis.OM;
using DragaliaAPI.Photon.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using DragaliaAPI.Photon.Shared.Enums;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Controller to export room state to callers.
/// </summary>
[Route("[controller]")]
[ApiController]
[AllowAnonymous]
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
    [ProducesResponseType(typeof(IEnumerable<ApiGame>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApiGame>>> GameList([FromQuery] int? questId)
    {
        IRedisCollection<RedisGame> query = this.connectionProvider
            .RedisCollection<RedisGame>()
            .Where(x => x.MatchingType == MatchingTypes.Anyone);

        if (questId is not null)
            query = query.Where(x => x.QuestId == questId);

        IEnumerable<ApiGame> games = (await query.ToListAsync()).Select(x => new ApiGame(x));

        return this.Ok(games);
    }

    [HttpGet("[action]/{roomId}")]
    public async Task<ActionResult<ApiGame>> ById(int roomId)
    {
        // Maybe this should filter by MatchingType.ById. Probably no harm letting it join any room for now though.
        IRedisCollection<RedisGame> query = this.connectionProvider
            .RedisCollection<RedisGame>()
            .Where(x => x.RoomId == roomId);

        RedisGame? game = await query.FirstOrDefaultAsync();
        if (game is null)
            return this.NotFound();

        return this.Ok(new ApiGame(game));
    }
}
