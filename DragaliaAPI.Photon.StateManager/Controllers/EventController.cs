using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Photon.Dto.Requests;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NReJSON;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Controller to receive incoming webhooks from Photon.
/// </summary>
[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly IConnectionMultiplexer connectionMultiplexer;
    private readonly IOptionsMonitor<RedisOptions> options;
    private readonly ILogger<EventController> logger;

    private TimeSpan KeyExpiry => TimeSpan.FromMinutes(this.options.CurrentValue.KeyExpiryTimeMins);

    /// <summary>
    /// Creates a new instance of the <see cref="EventController"/> class.
    /// </summary>
    /// <param name="connectionMultiplexer"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public EventController(
        IConnectionMultiplexer connectionMultiplexer,
        IOptionsMonitor<RedisOptions> options,
        ILogger<EventController> logger
    )
    {
        this.connectionMultiplexer = connectionMultiplexer;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// Register the creation of a game.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> GameCreate(GameCreateRequest request)
    {
        IDatabase database = connectionMultiplexer.GetDatabase();

        RedisKey gameKey = RedisSchema.Game(request.Game.Name);

        (await database.JsonSetAsync(gameKey, request.Game)).EnsureSuccess();
        (await database.KeyExpireAsync(gameKey, KeyExpiry)).EnsureSuccess();

        this.logger.LogInformation("Created new game: {@game}", request.Game);

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }

    /// <summary>
    /// Register a player joining a game.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> GameJoin(GameModifyRequest request)
    {
        IDatabase database = connectionMultiplexer.GetDatabase();
        RedisKey gameKey = RedisSchema.Game(request.GameName);

        List<Player> players = (
            await database.JsonGetAsync<List<Player>>(gameKey, JsonPaths.Game.Players)
        ).First();

        if (players.Count >= 4)
        {
            this.logger.LogError(
                "Player {@player} attempted to join full game {gameName}",
                request.Player,
                request.GameName
            );

            return this.Conflict();
        }

        players.Add(request.Player);
        (await database.JsonSetAsync(gameKey, players, JsonPaths.Game.Players)).EnsureSuccess();

        this.logger.LogInformation(
            "Added player {player} to game {@game}",
            request.Player.ViewerId,
            request.GameName
        );

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }

    /// <summary>
    /// Register a player leaving a game.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> GameLeave(GameModifyRequest request)
    {
        IDatabase database = connectionMultiplexer.GetDatabase();
        RedisKey gameKey = RedisSchema.Game(request.GameName);

        List<Player> players = (
            await database.JsonGetAsync<List<Player>>(gameKey, JsonPaths.Game.Players)
        ).First();

        Player? toRemove = players.FirstOrDefault(x => x.ViewerId == request.Player.ViewerId);
        if (toRemove is null)
        {
            this.logger.LogError(
                "Cannot remove player {@player} from game {gameName} with players {players} as they are not in it.",
                request.Player,
                players,
                request.GameName
            );

            return this.BadRequest();
        }

        players.Remove(toRemove);
        (await database.JsonSetAsync(gameKey, players, JsonPaths.Game.Players)).EnsureSuccess();

        this.logger.LogInformation(
            "Removed player {@player} from game {game}",
            request.Player,
            request.GameName
        );

        if (players.Count == 0)
        {
            // Don't remove it just yet, as Photon will request that shortly
            (await database.JsonSetAsync(gameKey, false, JsonPaths.Game.Visible)).EnsureSuccess();
        }

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }

    /// <summary>
    /// Register a game being closed.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> GameClose(GameModifyRequest request)
    {
        IDatabase database = connectionMultiplexer.GetDatabase();
        RedisKey gameKey = RedisSchema.Game(request.GameName);

        if (await database.JsonDeleteAsync(gameKey) < 1)
        {
            this.logger.LogError("Failed to remove game {gameName}", request.GameName);
        }
        else
        {
            this.logger.LogInformation("Removed game {gameName}", request.GameName);
        }

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }
}
