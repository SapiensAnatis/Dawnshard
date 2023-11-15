using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using DragaliaAPI.Photon.StateManager.Authentication;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Controller to receive incoming webhooks from Photon.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = nameof(PhotonAuthenticationHandler))]
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
    /// <param name="connectionProvider"></param>
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
    [HttpPost("GameCreate")]
    public async Task<IActionResult> GameCreate(GameCreateRequest request)
    {
        RedisGame newGame = new(request.Game);
        newGame.Players.Add(request.Player);

        await this.Games.InsertAsync(newGame, this.KeyExpiry);
        await this.Games.SaveAsync();

        this.logger.LogInformation("Created new game: {@game}", newGame);

        return this.Ok();
    }

    /// <summary>
    /// Register a player joining a game.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("GameJoin")]
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
                "Player {@player} attempted to join full game {@game}",
                request.Player,
                game
            );

            return this.Conflict();
        }

        if (game.Players.Any(x => x.ViewerId == request.Player.ViewerId))
        {
            this.logger.LogError(
                "Player {@player} attempted to join game {@game} that they were already in.",
                request.Player,
                game
            );

            return this.Conflict();
        }

        game.Players.Add(request.Player);
        await this.Games.UpdateAsync(game);

        this.logger.LogInformation("Added player {@player} to game {@game}", request.Player, game);

        return this.Ok();
    }

    /// <summary>
    /// Register a player leaving a game.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("GameLeave")]
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
            this.logger.LogInformation(
                "Player {@player} was not in game {@game}",
                request.Player,
                game
            );

            return this.Ok();
        }

        game.Players.Remove(toRemove);
        if (game.Players.Count == 0 || request.Player.ActorNr == 1)
        {
            // Don't remove it just yet, as Photon will request that shortly
            this.logger.LogDebug("Hiding game {@game}", game);
            game.Visible = false;
        }

        await this.Games.UpdateAsync(game);

        this.logger.LogInformation(
            "Removed player {@player} from game {@game}",
            request.Player,
            game
        );

        return this.Ok();
    }

    /// <summary>
    /// Register a game being closed.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A webhook response.</returns>
    [HttpPost("GameClose")]
    public async Task<IActionResult> GameClose(GameModifyRequest request)
    {
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            this.logger.LogInformation(
                "Could not find game {name}. It may have already been closed.",
                request.GameName
            );

            return this.Ok();
        }

        await this.Games.DeleteAsync(game);
        this.logger.LogInformation("Removed game {@game}", game);

        return this.Ok();
    }

    /// <summary>
    /// Register a game's entry conditions being changed.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A HTTP response.</returns>
    [HttpPost("EntryConditions")]
    public async Task<IActionResult> EntryConditions(GameModifyConditionsRequest request)
    {
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            this.logger.LogError("Could not find game {name}", request.GameName);
            return this.NotFound();
        }

        game.EntryConditions = request.NewEntryConditions;
        await this.Games.UpdateAsync(game);

        this.logger.LogInformation(
            "Updated game {game} entry conditions to {@conditions}",
            game.Name,
            game.EntryConditions
        );

        return this.Ok();
    }

    /// <summary>
    /// Register a game's matching type (i.e. accessibility) being changed.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A HTTP response.</returns>
    [HttpPost("MatchingType")]
    public async Task<IActionResult> MatchingType(GameModifyMatchingTypeRequest request)
    {
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            this.logger.LogError("Could not find game {name}", request.GameName);
            return this.NotFound();
        }

        game.MatchingType = request.NewMatchingType;
        await this.Games.UpdateAsync(game);

        this.logger.LogInformation(
            "Updated game {game} matching type to {type}",
            game.Name,
            game.MatchingType
        );

        return this.Ok();
    }

    /// <summary>
    /// Register a game's room ID being changed.
    /// </summary>
    /// <param name="request">The webhook data.</param>
    /// <returns>A HTTP response.</returns>
    [HttpPost("RoomId")]
    public async Task<IActionResult> RoomId(GameModifyRoomIdRequest request)
    {
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            this.logger.LogError("Could not find game {name}", request.GameName);
            return this.NotFound();
        }

        game.RoomId = request.NewRoomId;
        await this.Games.UpdateAsync(game);

        this.logger.LogInformation(
            "Updated game {game} room ID to {newId}",
            game.Name,
            game.RoomId
        );

        return this.Ok();
    }

    [HttpPost("Visible")]
    public async Task<IActionResult> Visible(GameModifyVisibleRequest request)
    {
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            this.logger.LogError("Could not find game {name}", request.GameName);
            return this.NotFound();
        }

        game.Visible = request.NewVisibility;
        await this.Games.UpdateAsync(game);

        this.logger.LogInformation(
            "Updated game {game} visibility to {newVisibility}",
            game.Name,
            game.Visible
        );

        return this.Ok();
    }
}
