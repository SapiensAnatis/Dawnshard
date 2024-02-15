using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public record EventData(
    int Id,
    [property: JsonConverter(typeof(BoolIntJsonConverter))] bool IsMemoryEvent,
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
);
