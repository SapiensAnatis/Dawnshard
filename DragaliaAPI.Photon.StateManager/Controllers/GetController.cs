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
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(IEnumerable<ApiGame>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApiGame>>> GameList([FromQuery] int? questId)
    {
        this.logger.LogDebug("Retrieving all open games.");

        IRedisCollection<RedisGame> query = this.VisibleGames.Where(
            x => x.MatchingType == MatchingTypes.Anyone
        );

        if (questId is not null)
        {
            this.logger.LogDebug("Filtering by quest ID {id}", questId);
            query = query.Where(x => x.QuestId == questId);
        }

        IEnumerable<ApiGame> games = (await query.ToListAsync())
            .Select(x => new ApiGame(x))
            .ToList();

        this.logger.LogDebug("Found {n} games", games.Count());

        return this.Ok(games);
    }

    /// <summary>
    /// Get a room by its room ID.
    /// </summary>
    /// <param name="roomId">The room ID.</param>
    /// <returns>A room with that ID, or a 404 if not found.</returns>
    [HttpGet("[action]/{roomId}")]
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
        return this.Ok(new ApiGame(game));
    }

    /// <summary>
    /// Get a value indicating whether the given <paramref name="viewerId"/> is a host in any room.
    /// </summary>
    /// <param name="viewerId">The viewer ID.</param>
    /// <returns>True if a host, false if not.</returns>
    [HttpGet("[action]/{viewerId}")]
    public async Task<ActionResult<bool>> IsHost(long viewerId)
    {
        this.logger.LogDebug("Checking whether player {viewerId} is a host in any game", viewerId);

        bool result = (await this.Games.ToListAsync()).Any(
            x => x.Players.Any(y => y.ViewerId == viewerId && y.ActorNr == 1)
        );

        this.logger.LogDebug("Result: {result}", result);

        return this.Ok(result);
    }

    /// <summary>
    /// Get the room that the given <paramref name="viewerId"/> is playing in.
    /// </summary>
    /// <param name="viewerId">The viewer ID.</param>
    /// <returns>The room they are in, or 404 if not found.</returns>
    [HttpGet("[action]/{viewerId}")]
    public async Task<ActionResult<ApiGame>> ByViewerId(long viewerId)
    {
        this.logger.LogDebug("Searching for game containing player {viewerId}", viewerId);

        RedisGame? game = (
            await this.Games.OrderByDescending(x => x.StartEntryTimestamp).ToListAsync()
        ).FirstOrDefault(x => x.Players.Any(x => x.ViewerId == viewerId));

        if (game is null)
        {
            this.logger.LogDebug("Could not find any game with given player.");
            return this.NotFound("No game found.");
        }

        this.logger.LogDebug("Found player in game {@game}", game);
        return new ApiGame(game);
    }
}
