using DragaliaAPI.Photon.Shared.Enums;

namespace DragaliaAPI.Photon.Shared.Models;

/// <summary>
/// DTO for game objects to be created by Photon.
/// </summary>
public class GameBase : IGame
{
    /// <inheritdoc/>
    public int RoomId { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc/>
    public int MatchingCompatibleId { get; set; }

    /// <inheritdoc/>
    public MatchingTypes MatchingType { get; set; }

    /// <inheritdoc/>
    public int QuestId { get; set; }

    /// <inheritdoc/>
    public EntryConditions EntryConditions { get; set; } = new EntryConditions();

    /// <inheritdoc/>
    public DateTimeOffset StartEntryTime { get; set; } = DateTimeOffset.UtcNow;

    /// <inheritdoc/>
    public List<Player> Players { get; set; } = new List<Player>();
}
