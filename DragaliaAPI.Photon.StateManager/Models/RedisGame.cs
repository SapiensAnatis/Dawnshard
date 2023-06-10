using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Photon.Dto.Game;
using Newtonsoft.Json;
using Redis.OM.Modeling;

namespace DragaliaAPI.Photon.StateManager.Models;

/// <summary>
/// Data transfer object for a game to be sent by Photon and stored in Redis.
/// </summary>
[Document(StorageType = StorageType.Json)]
public class RedisGame : IGame
{
    /// <summary>
    /// The room id / passcode.
    /// </summary>
    [Indexed]
    public int RoomId { get; set; }

    /// <summary>
    /// The unique name of the game.
    /// </summary>
    [RedisIdField]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The compatibility ID of this game.
    /// </summary>
    [Indexed]
    public int MatchingCompatibleId { get; set; }

    /// <summary>
    /// The quest ID for this game.
    /// </summary>
    [Indexed]
    public int QuestId { get; set; }

    /// <summary>
    /// The time the room was opened.
    /// </summary>
    public DateTimeOffset StartEntryTime { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The entry conditions attached to this room.
    /// </summary>
    public EntryConditions EntryConditions { get; set; } = new EntryConditions();

    /// <summary>
    /// The list of players in this game.
    /// </summary>
    public List<Player> Players { get; set; } = new List<Player>();

    /// <summary>
    /// Whether the room should be shown to players.
    /// </summary>
    [Indexed]
    public bool Visible { get; set; } = true;

    public RedisGame(IGame game)
    {
        RoomId = game.RoomId;
        Name = game.Name;
        MatchingCompatibleId = game.MatchingCompatibleId;
        QuestId = game.QuestId;
        StartEntryTime = game.StartEntryTime;
        EntryConditions = game.EntryConditions;
        Players = game.Players;
    }

    [JsonConstructor]
    public RedisGame() { }
}
