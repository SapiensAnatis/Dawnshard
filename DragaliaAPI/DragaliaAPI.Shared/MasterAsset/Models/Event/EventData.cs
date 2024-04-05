using System.Collections.Frozen;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public record EventData(
    int Id,
    bool IsMemoryEvent,
    EventKindType EventKindType,
    FortPlants EventFortId,
    EntityTypes ViewEntityType1,
    int ViewEntityId1,
    EntityTypes ViewEntityType2,
    int ViewEntityId2,
    EntityTypes ViewEntityType3,
    int ViewEntityId3,
    EntityTypes ViewEntityType4,
    int ViewEntityId4,
    EntityTypes ViewEntityType5,
    int ViewEntityId5,
    Charas EventCharaId,
    int GuestJoinStoryId
)
{
    /// <summary>
    /// Dictionary of [EventId, QuestStoryId] overrides for GuestJoinStoryId, where the master asset data is incorrect.
    /// </summary>
    private static readonly FrozenDictionary<int, int> OverrideGuestJoinStoryId = new Dictionary<
        int,
        int
    >()
    {
        [20443] = 2044303, // Faith Forsaken Part One / Harle / Originally 2043803
        [20462] = 2046203, // Advent of the Origin    / Origa / Originally 2043803
    }.ToFrozenDictionary();

    public int GetActualGuestJoinStoryId() =>
        OverrideGuestJoinStoryId.GetValueOrDefault(this.Id, this.GuestJoinStoryId);
}
