using System.Text.Json.Serialization;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using Redis.OM.Modeling;

namespace DragaliaAPI.Photon.StateManager.Models;

/// <summary>
/// Implementation of <see cref="IGame"/> with Redis metadata.
/// </summary>
[Document(StorageType = StorageType.Json)]
public class RedisGame : IGame
{
    /// <inheritdoc/>
    [Indexed]
    public int RoomId { get; set; }

    /// <inheritdoc/>
    [RedisIdField]
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc/>
    [Indexed]
    public int MatchingCompatibleId { get; set; }

    /// <inheritdoc/>
    [Indexed(Sortable = true)]
    public MatchingTypes MatchingType { get; set; }

    /// <inheritdoc/>
    [Indexed]
    public int QuestId { get; set; }

    /// <summary>
    /// Determines whether a game should be visible or not. Redis-only property.
    /// </summary>
    [Indexed]
    public bool Visible { get; set; } = true;

    /// <inheritdoc/>
    public DateTimeOffset StartEntryTime { get; set; } = DateTimeOffset.UtcNow;

    [Indexed(Sortable = true)]
    public long StartEntryTimestamp => StartEntryTime.ToUnixTimeSeconds();

    /// <inheritdoc/>
    public EntryConditions EntryConditions { get; set; } = new EntryConditions();

    /// <inheritdoc/>
    [Indexed(CascadeDepth = 1)]
    public List<Player> Players { get; set; } = new List<Player>();

    /// <summary>
    /// Create a new instance of the <see cref="RedisGame"/> class.
    /// </summary>
    /// <param name="game">Base game, e.g. <see cref="GameBase"/>.</param>
    public RedisGame(IGame game)
    {
        RoomId = game.RoomId;
        Name = game.Name;
        MatchingCompatibleId = game.MatchingCompatibleId;
        QuestId = game.QuestId;
        StartEntryTime = game.StartEntryTime;
        EntryConditions = game.EntryConditions;
        Players = game.Players;
        MatchingType = game.MatchingType;
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    [JsonConstructor]
    public RedisGame() { }
}
