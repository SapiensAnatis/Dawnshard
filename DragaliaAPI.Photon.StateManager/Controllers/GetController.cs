using System.Reflection;
using DragaliaAPI.Photon.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Controller to export room state to callers.
/// </summary>
[Route("[controller]")]
[ApiController]
public class GetController : ControllerBase
{
    private readonly IConnectionMultiplexer connectionMultiplexer;

    public GetController(IConnectionMultiplexer connectionMultiplexer)
    {
        this.connectionMultiplexer = connectionMultiplexer;
    }

    /// <summary>
    /// Get a list of all open games.
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(IEnumerable<Game>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GameList()
    {
        IDatabase db = this.connectionMultiplexer.GetDatabase();

        RedisKey gameList = RedisSchema.GameList();
        IEnumerable<string> gameNames = (await db.SetMembersAsync(gameList)).Select(
            x => x.ToString()
        );

        return this.Ok(this.GetGameList(gameNames));
    }

    private async IAsyncEnumerable<Game> GetGameList(IEnumerable<string> gameNames)
    {
        IDatabase db = this.connectionMultiplexer.GetDatabase();

        foreach (string name in gameNames)
        {
            Game? game = await GetGame(db, name);

            if (game is not null)
                yield return game;
        }
    }

    private static async Task<Game?> GetGame(IDatabase db, string gameName)
    {
        RedisKey gameInfo = RedisSchema.Game(gameName);
        RedisKey gamePlayers = RedisSchema.GamePlayers(gameName);

        HashEntry[] gameInfoEntries = await db.HashGetAllAsync(gameInfo);

        if (!gameInfoEntries.Any())
            return null;

        Dictionary<string, RedisValue> infoEntries = gameInfoEntries.ToDictionary(
            x => x.Name.ToString(),
            x => x.Value
        );

        Game result = new();

        foreach (PropertyInfo property in typeof(Game).GetProperties())
        {
            if (!property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                continue;

            RedisValue v = infoEntries[property.Name];
            property.SetValue(result, Convert.ChangeType(v, property.PropertyType));
        }

        result.Players = (await db.SetMembersAsync(gamePlayers)).Select(x => (int)x);

        return result;
    }
}
