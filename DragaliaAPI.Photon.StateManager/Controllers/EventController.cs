using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Photon.Dto.Requests;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NReJSON;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Controller to receive incoming webhooks from Photon.
/// </summary>
[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly IOptionsMonitor<RedisOptions> options;
    private readonly IRedisConnectionProvider connectionProvider;
    private readonly ILogger<EventController> logger;

    private TimeSpan KeyExpiry => TimeSpan.FromMinutes(this.options.CurrentValue.KeyExpiryTimeMins);
    private IRedisCollection<RedisGame> Games =>
        this.connectionProvider.RedisCollection<RedisGame>();

    /// <summary>
    /// Creates a new instance of the <see cref="EventController"/> class.
    /// </summary>
    /// <param name="connectionMultiplexer"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public EventController(
        IRedisConnectionProvider connectionProvider,
        IOptionsMonitor<RedisOptions> options,
        ILogger<EventController> logger
    )
    {
        this.options = options;
        this.connectionProvider = connectionProvider;
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
        RedisGame newGame = new(request.Game);

        await this.Games.InsertAsync(newGame, this.KeyExpiry);
        this.logger.LogInformation("Created new game: {@game}", newGame);

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
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            this.logger.LogError("Could not find game {name}", request.GameName);
            return this.NotFound();
        }

        if (game.Players.Count >= 4)
        {
            this.logger.LogError(
                "Player {@player} attempted to join full game {gameName}",
                request.Player,
                request.GameName
            );

            return this.Conflict();
        }

        game.Players.Add(request.Player);
        await this.Games.UpdateAsync(game);

        this.logger.LogInformation("Added player {@player} to game {@game}", request.Player, game);

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
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            this.logger.LogError("Could not find game {name}", request.GameName);
            return this.NotFound();
        }

        Player? toRemove = game.Players.FirstOrDefault(x => x.ViewerId == request.Player.ViewerId);
        if (toRemove is null)
        {
            this.logger.LogError(
                "Cannot remove player {@player} from game {gameName} with players {players} as they are not in it.",
                request.Player,
                game.Players,
                request.GameName
            );

            return this.BadRequest();
        }

        game.Players.Remove(toRemove);
        if (game.Players.Count == 0)
        {
            // Don't remove it just yet, as Photon will request that shortly
            game.Visible = false;
        }

        await this.Games.UpdateAsync(game);

        this.logger.LogInformation(
            "Removed player {@player} from game {@game}",
            request.Player,
            game
        );

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
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            this.logger.LogError("Could not find game {name}", request.GameName);
            return this.NotFound();
        }

        await this.Games.DeleteAsync(game);
        this.logger.LogInformation("Removed game {@game}", game);

        return this.Ok(new WebhookResponse() { ResultCode = 0 });
    }
}
