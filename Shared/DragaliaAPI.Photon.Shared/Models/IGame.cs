using DragaliaAPI.Photon.Shared.Enums;

namespace DragaliaAPI.Photon.Shared.Models;

/// <summary>
/// Interface for classes such as <see cref="GameBase"/>
/// </summary>
public interface IGame
{
    /// <summary>
    /// The room id / passcode.
    /// </summary>
    int RoomId { get; set; }

    /// <summary>
    /// The unique name of the game.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// The compatibility ID of this game.
    /// </summary>
    int MatchingCompatibleId { get; set; }

    /// <summary>
    /// The matching type of this game (e.g. open/private/guild)
    /// </summary>
    MatchingTypes MatchingType { get; set; }

    /// <summary>
    /// The quest ID for this game.
    /// </summary>
    int QuestId { get; set; }

    /// <summary>
    /// The time the room was opened.
    /// </summary>
    DateTimeOffset StartEntryTime { get; set; }

    /// <summary>
    /// The entry conditions attached to this room.
    /// </summary>
    EntryConditions EntryConditions { get; set; }

    /// <summary>
    /// The list of players in this game.
    /// </summary>

    List<Player> Players { get; set; }
}
