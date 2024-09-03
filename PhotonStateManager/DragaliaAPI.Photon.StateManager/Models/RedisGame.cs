using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using Redis.OM.Modeling;

namespace DragaliaAPI.Photon.StateManager.Models;

/// <summary>
/// Represents a game as stored in Redis.
/// </summary>
[Document(StorageType = StorageType.Json)]
public class RedisGame
{
    /// <summary>
    /// Create a new instance of the <see cref="RedisGame"/> class.
    /// </summary>
    /// <param name="game">Base game, e.g. <see cref="GameBase"/>.</param>
    [SetsRequiredMembers]
    public RedisGame(IGame game)
    {
        Name = game.Name;
        RoomId = game.RoomId;
        MatchingCompatibleId = game.MatchingCompatibleId;
        QuestId = game.QuestId;
        StartEntryTime = game.StartEntryTime;
        EntryConditions = game.EntryConditions;
        Players = game.Players.Select(p => new RedisPlayer(p)).ToList();
        MatchingType = game.MatchingType;
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    [JsonConstructor]
    public RedisGame() { }

    /// <summary>
    /// Gets or sets the unique room name / identifier.
    /// </summary>
    [RedisIdField]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the room passcode.
    /// </summary>
    [Indexed]
    public int RoomId { get; set; }

    /// <summary>
    /// Gets or sets the room version compatibility ID.
    /// </summary>
    [Indexed]
    public int MatchingCompatibleId { get; set; }

    /// <summary>
    /// Gets or sets the room privacy setting.
    /// </summary>
    [Indexed(Sortable = true)]
    public MatchingTypes MatchingType { get; set; }

    /// <summary>
    /// Gets or sets the room's quest ID.
    /// </summary>
    [Indexed]
    public int QuestId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a game should be visible or not. Redis-only property.
    /// </summary>
    [Indexed]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the starting time of the room.
    /// </summary>
    public DateTimeOffset StartEntryTime { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the room's requirements to join.
    /// </summary>
    public required EntryConditions EntryConditions { get; set; }

    /// <summary>
    /// Gets or sets a list of players in the room.
    /// </summary>
    [Indexed(JsonPath = $"$.{nameof(RedisPlayer.ViewerId)}")]
    public required List<RedisPlayer> Players { get; set; }

    /// <summary>
    /// Maps a game to a <see cref="ApiGame"/>.
    /// </summary>
    /// <returns>An instance of <see cref="ApiGame"/>.</returns>
    public ApiGame ToApiGame() =>
        new()
        {
            Name = this.Name,
            RoomId = this.RoomId,
            MatchingCompatibleId = this.MatchingCompatibleId,
            MatchingType = this.MatchingType,
            QuestId = this.QuestId,
            StartEntryTime = this.StartEntryTime,
            EntryConditions = this.EntryConditions,
            Players = this.Players.Select(x => x.ToPlayer()).ToList(),
        };
}
