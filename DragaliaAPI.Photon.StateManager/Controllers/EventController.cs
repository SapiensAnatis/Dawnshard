using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog.Parsing;
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
    public async Task<IActionResult> GameCreate(WebhookRequest request)
    {
        this.logger.LogDebug("Received GameCreate request: {@request}", request);

        IDatabase database = connectionMultiplexer.GetDatabase();
        RedisKey gameInfo = RedisSchema.GameInfo(request.Game.Name);
        RedisKey gamePlayers = RedisSchema.GamePlayers(request.Game.Name);
        RedisKey gameList = RedisSchema.GameList();

        List<HashEntry> entries =
            new() { new(nameof(StoredGame.HostViewerId), request.Player.ViewerId), };

        foreach (System.Reflection.PropertyInfo property in typeof(GameDto).GetProperties())
        {
            RedisValue value = property.GetValue(request.Game) switch
            {
                string s => s,
                int i => i,
                _
                    => throw new NotSupportedException(
                        $"Values of type {property.GetType()} cannot be stored in Redis hashes"
                    )
            };

            entries.Add(new(property.Name, value));
        }

        await database.HashSetAsync(gameInfo, entries.ToArray());

        await database.SetAddAsync(gamePlayers, request.Player.ViewerId);
        await database.SetAddAsync(gameList, request.Game.Name);

        await database.KeyExpireAsync(gameInfo, KeyExpiry);
        await database.KeyExpireAsync(gamePlayers, KeyExpiry);

        this.logger.LogInformation(
            "Created new game {name} for player {player}",
            request.Game.Name,
            request.Player.ViewerId
        );

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }

    /// <summary>
    /// Register a player joining a game.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> GameJoin(WebhookRequest request)
    {
        this.logger.LogDebug("Received GameCreate request: {@request}", request);

        IDatabase database = connectionMultiplexer.GetDatabase();
        RedisKey gamePlayers = RedisSchema.GamePlayers(request.Game.Name);

        await database.SetAddAsync(gamePlayers, request.Player.ViewerId);

        this.logger.LogInformation(
            "Added player {player} to game {game}",
            request.Player.ViewerId,
            request.Game.Name
        );

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }

    /// <summary>
    /// Register a player leaving a game.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> GameLeave(WebhookRequest request)
    {
        this.logger.LogDebug("Received GameLeave request: {@request}", request);

        IDatabase database = connectionMultiplexer.GetDatabase();
        RedisKey gamePlayers = RedisSchema.GamePlayers(request.Game.Name);

        await database.SetRemoveAsync(gamePlayers, request.Player.ViewerId);

        this.logger.LogInformation(
            "Removed player {player} from game {game}",
            request.Player.ViewerId,
            request.Game.Name
        );

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }

    /// <summary>
    /// Register a game being closed.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> GameClose(WebhookRequest request)
    {
        this.logger.LogDebug("Received GameClose request: {@request}", request);

        IDatabase database = connectionMultiplexer.GetDatabase();
        RedisKey gameList = RedisSchema.GameList();
        RedisKey gameInfo = RedisSchema.GameInfo(request.Game.Name);
        RedisKey gamePlayers = RedisSchema.GamePlayers(request.Game.Name);

        await database.KeyDeleteAsync(new[] { gameInfo, gamePlayers });
        await database.SetRemoveAsync(gameList, request.Game.Name);

        this.logger.LogInformation("Removed game {game}", request.Game.Name);

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }
}
