using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Controller to export room state to callers.
/// </summary>
[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public partial class GetController : ControllerBase
{
    private readonly IRedisConnectionProvider connectionProvider;
    private readonly ILogger<GetController> logger;

    private IRedisCollection<RedisGame> Games =>
        this.connectionProvider.RedisCollection<RedisGame>();

    private IRedisCollection<RedisGame> VisibleGames =>
        this.Games.Where(x => x.Visible == true && x.RoomId > 0);

    public GetController(IRedisConnectionProvider connectionProvider, ILogger<GetController> logger)
    {
        this.connectionProvider = connectionProvider;
        this.logger = logger;
    }

    /// <summary>
    /// Get a list of all open games.
    /// </summary>
    /// <returns>A list of games.</returns>
    [HttpGet("GameList")]
    [ProducesResponseType(typeof(IEnumerable<ApiGame>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApiGame>>> GameList([FromQuery] int? questId)
    {
        Log.RetrievingAllOpenGames(this.logger);

        IRedisCollection<RedisGame> query = this.VisibleGames.Where(x =>
            x.MatchingType == MatchingTypes.Anyone
        );

        if (questId is not null)
        {
            Log.FilteringByQuestID(this.logger, questId);
            query = query.Where(x => x.QuestId == questId);
        }

        IEnumerable<ApiGame> games = (await query.ToListAsync())
            .Select(x => x.ToApiGame())
            .ToList();

        Log.FoundGames(this.logger, games.Count());

        return this.Ok(games);
    }

    /// <summary>
    /// Get a room by its room ID.
    /// </summary>
    /// <param name="roomId">The room ID.</param>
    /// <returns>A room with that ID, or a 404 if not found.</returns>
    [HttpGet("ById/{roomId}")]
    public async Task<ActionResult<ApiGame>> ById(int roomId)
    {
        Log.SearchingForGamesWithID(this.logger, roomId);

        RedisGame? game = await this.VisibleGames.FirstOrDefaultAsync(x => x.RoomId == roomId);

        if (game is null)
        {
            Log.GameNotFound(this.logger);
            return this.NotFound();
        }

        Log.FoundGame(this.logger, game);
        return this.Ok(game.ToApiGame());
    }

    /// <summary>
    /// Get a value indicating whether the given <paramref name="viewerId"/> is a host in any room.
    /// </summary>
    /// <param name="viewerId">The viewer ID.</param>
    /// <returns>True if a host, false if not.</returns>
    [HttpGet("IsHost/{viewerId}")]
    public async Task<ActionResult<bool>> IsHost(long viewerId)
    {
        Log.CheckingWhetherPlayerIsAHostInAnyGame(this.logger, viewerId);

        RedisGame? game = await this.GetPlayerGame(viewerId);

        if (game is null)
        {
            Log.GameNotFound(this.logger);
            return false;
        }

        // Cannot do this query as part of the expression: https://github.com/redis/redis-om-dotnet/issues/461
        bool isHost = game.Players.FirstOrDefault(x => x.ViewerId == viewerId)?.ActorNr == 1;

        Log.Result(this.logger, isHost);

        return this.Ok(isHost);
    }

    /// <summary>
    /// Get the room that the given <paramref name="viewerId"/> is playing in.
    /// </summary>
    /// <param name="viewerId">The viewer ID.</param>
    /// <returns>The room they are in, or 404 if not found.</returns>
    [HttpGet("ByViewerId/{viewerId}")]
    public async Task<ActionResult<ApiGame>> ByViewerId(long viewerId)
    {
        Log.SearchingForGameContainingPlayer(this.logger, viewerId);

        RedisGame? game = await this.GetPlayerGame(viewerId);

        if (game is null)
        {
            Log.CouldNotFindAnyGameWithGivenPlayer(this.logger);
            return this.NotFound("No game found.");
        }

        Log.FoundPlayerInGame(this.logger, game);
        return game.ToApiGame();
    }

    private Task<RedisGame?> GetPlayerGame(long viewerId) =>
        this.Games.Where(x => x.Players.Any(y => y.ViewerId == viewerId)).FirstOrDefaultAsync();

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Retrieving all open games.")]
        public static partial void RetrievingAllOpenGames(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Filtering by quest ID {id}")]
        public static partial void FilteringByQuestID(ILogger logger, int? id);

        [LoggerMessage(LogLevel.Debug, "Found {n} games")]
        public static partial void FoundGames(ILogger logger, int n);

        [LoggerMessage(LogLevel.Debug, "Searching for games with ID {roomId}")]
        public static partial void SearchingForGamesWithID(ILogger logger, int roomId);

        [LoggerMessage(LogLevel.Debug, "Game not found.")]
        public static partial void GameNotFound(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Found game: {@game}")]
        public static partial void FoundGame(ILogger logger, RedisGame game);

        [LoggerMessage(LogLevel.Debug, "Checking whether player {viewerId} is a host in any game")]
        public static partial void CheckingWhetherPlayerIsAHostInAnyGame(
            ILogger logger,
            long viewerId
        );

        [LoggerMessage(LogLevel.Debug, "Result: {result}")]
        public static partial void Result(ILogger logger, bool result);

        [LoggerMessage(LogLevel.Debug, "Searching for game containing player {viewerId}")]
        public static partial void SearchingForGameContainingPlayer(ILogger logger, long viewerId);

        [LoggerMessage(LogLevel.Debug, "Could not find any game with given player.")]
        public static partial void CouldNotFindAnyGameWithGivenPlayer(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Found player in game {@game}")]
        public static partial void FoundPlayerInGame(ILogger logger, RedisGame game);
    }
}
