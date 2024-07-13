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
public class GetController : ControllerBase
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
        this.logger.LogDebug("Retrieving all open games.");

        IRedisCollection<RedisGame> query = this.VisibleGames.Where(x =>
            x.MatchingType == MatchingTypes.Anyone
        );

        if (questId is not null)
        {
            this.logger.LogDebug("Filtering by quest ID {id}", questId);
            query = query.Where(x => x.QuestId == questId);
        }

        IEnumerable<ApiGame> games = (await query.ToListAsync())
            .Select(x => x.ToApiGame())
            .ToList();

        this.logger.LogDebug("Found {n} games", games.Count());

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
        this.logger.LogDebug("Searching for games with ID {roomId}", roomId);

        RedisGame? game = await this.VisibleGames.FirstOrDefaultAsync(x => x.RoomId == roomId);

        if (game is null)
        {
            this.logger.LogDebug("Game not found.");
            return this.NotFound();
        }

        this.logger.LogDebug("Found game: {@game}", game);
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
        this.logger.LogDebug("Checking whether player {viewerId} is a host in any game", viewerId);

        RedisGame? game = await this.GetPlayerGame(viewerId);

        if (game is null)
        {
            this.logger.LogDebug("Game not found.");
            return false;
        }

        // Cannot do this query as part of the expression: https://github.com/redis/redis-om-dotnet/issues/461
        bool isHost = game.Players.FirstOrDefault(x => x.ViewerId == viewerId)?.ActorNr == 1;

        this.logger.LogDebug("Result: {result}", isHost);

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
        this.logger.LogDebug("Searching for game containing player {viewerId}", viewerId);

        RedisGame? game = await this.GetPlayerGame(viewerId);

        if (game is null)
        {
            this.logger.LogDebug("Could not find any game with given player.");
            return this.NotFound("No game found.");
        }

        this.logger.LogDebug("Found player in game {@game}", game);
        return game.ToApiGame();
    }

    private Task<RedisGame?> GetPlayerGame(long viewerId) =>
        this.Games.Where(x => x.Players.Any(y => y.ViewerId == viewerId)).FirstOrDefaultAsync();
}
