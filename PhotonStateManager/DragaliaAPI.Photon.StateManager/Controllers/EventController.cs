using DragaliaAPI.Photon.Shared.Enums;
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
public partial class EventController : ControllerBase
{
    private readonly IOptionsMonitor<RedisCachingOptions> options;
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
        IOptionsMonitor<RedisCachingOptions> options,
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
        newGame.Players.Add(new(request.Player));

        await this.Games.InsertAsync(newGame, this.KeyExpiry);
        await this.Games.SaveAsync();

        Log.CreatedNewGame(this.logger, newGame);

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
            Log.CouldNotFindGame(this.logger, request.GameName);
            return this.NotFound();
        }

        if (game.Players.Count >= 4)
        {
            Log.PlayerAttemptedToJoinFullGame(this.logger, request.Player, game);

            return this.Conflict();
        }

        if (game.Players.Any(x => x.ViewerId == request.Player.ViewerId))
        {
            Log.PlayerAttemptedToJoinGameThatTheyWereAlreadyIn(this.logger, request.Player, game);

            return this.Conflict();
        }

        game.Players.Add(new(request.Player));
        await this.Games.UpdateAsync(game);

        Log.AddedPlayerToGame(this.logger, request.Player, game);

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
            Log.CouldNotFindGame(this.logger, request.GameName);
            return this.NotFound();
        }

        RedisPlayer? toRemove = game.Players.FirstOrDefault(x =>
            x.ViewerId == request.Player.ViewerId
        );

        if (toRemove is null)
        {
            Log.PlayerWasNotInGame(this.logger, request.Player, game);

            return this.Ok();
        }

        game.Players.Remove(toRemove);
        if (game.Players.Count == 0 || request.Player.ActorNr == 1)
        {
            // Don't remove it just yet, as Photon will request that shortly
            Log.HidingGame(this.logger, game);
            game.Visible = false;
        }

        await this.Games.UpdateAsync(game);

        Log.RemovedPlayerFromGame(this.logger, request.Player, game);

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
            Log.CouldNotFindGameItMayHaveAlreadyBeenClosed(this.logger, request.GameName);

            return this.Ok();
        }

        await this.Games.DeleteAsync(game);
        Log.RemovedGame(this.logger, game);

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
            Log.CouldNotFindGame(this.logger, request.GameName);
            return this.NotFound();
        }

        game.EntryConditions = request.NewEntryConditions;
        await this.Games.UpdateAsync(game);

        Log.UpdatedGameEntryConditionsTo(this.logger, game.Name, game.EntryConditions);

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
            Log.CouldNotFindGame(this.logger, request.GameName);
            return this.NotFound();
        }

        game.MatchingType = request.NewMatchingType;
        await this.Games.UpdateAsync(game);

        Log.UpdatedGameMatchingTypeTo(this.logger, game.Name, game.MatchingType);

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
            Log.CouldNotFindGame(this.logger, request.GameName);
            return this.NotFound();
        }

        game.RoomId = request.NewRoomId;
        await this.Games.UpdateAsync(game);

        Log.UpdatedGameRoomIDTo(this.logger, game.Name, game.RoomId);

        return this.Ok();
    }

    [HttpPost("Visible")]
    public async Task<IActionResult> Visible(GameModifyVisibleRequest request)
    {
        RedisGame? game = await this.Games.FindByIdAsync(request.GameName);

        if (game is null)
        {
            Log.CouldNotFindGame(this.logger, request.GameName);
            return this.NotFound();
        }

        game.Visible = request.NewVisibility;
        await this.Games.UpdateAsync(game);

        Log.UpdatedGameVisibilityTo(this.logger, game.Name, game.Visible);

        return this.Ok();
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Created new game: {@game}")]
        public static partial void CreatedNewGame(ILogger logger, RedisGame game);

        [LoggerMessage(LogLevel.Error, "Could not find game {name}")]
        public static partial void CouldNotFindGame(ILogger logger, string name);

        [LoggerMessage(LogLevel.Error, "Player {@player} attempted to join full game {@game}")]
        public static partial void PlayerAttemptedToJoinFullGame(
            ILogger logger,
            Player player,
            RedisGame game
        );

        [LoggerMessage(
            LogLevel.Error,
            "Player {@player} attempted to join game {@game} that they were already in."
        )]
        public static partial void PlayerAttemptedToJoinGameThatTheyWereAlreadyIn(
            ILogger logger,
            Player player,
            RedisGame game
        );

        [LoggerMessage(LogLevel.Information, "Added player {@player} to game {@game}")]
        public static partial void AddedPlayerToGame(ILogger logger, Player player, RedisGame game);

        [LoggerMessage(LogLevel.Information, "Player {@player} was not in game {@game}")]
        public static partial void PlayerWasNotInGame(
            ILogger logger,
            Player player,
            RedisGame game
        );

        [LoggerMessage(LogLevel.Debug, "Hiding game {@game}")]
        public static partial void HidingGame(ILogger logger, RedisGame game);

        [LoggerMessage(LogLevel.Information, "Removed player {@player} from game {@game}")]
        public static partial void RemovedPlayerFromGame(
            ILogger logger,
            Player player,
            RedisGame game
        );

        [LoggerMessage(
            LogLevel.Information,
            "Could not find game {name}. It may have already been closed."
        )]
        public static partial void CouldNotFindGameItMayHaveAlreadyBeenClosed(
            ILogger logger,
            string name
        );

        [LoggerMessage(LogLevel.Information, "Removed game {@game}")]
        public static partial void RemovedGame(ILogger logger, RedisGame game);

        [LoggerMessage(
            LogLevel.Information,
            "Updated game {game} entry conditions to {@conditions}"
        )]
        public static partial void UpdatedGameEntryConditionsTo(
            ILogger logger,
            string game,
            EntryConditions conditions
        );

        [LoggerMessage(LogLevel.Information, "Updated game {game} matching type to {type}")]
        public static partial void UpdatedGameMatchingTypeTo(
            ILogger logger,
            string game,
            MatchingTypes type
        );

        [LoggerMessage(LogLevel.Information, "Updated game {game} room ID to {newId}")]
        public static partial void UpdatedGameRoomIDTo(ILogger logger, string game, int newId);

        [LoggerMessage(LogLevel.Information, "Updated game {game} visibility to {newVisibility}")]
        public static partial void UpdatedGameVisibilityTo(
            ILogger logger,
            string game,
            bool newVisibility
        );
    }
}
