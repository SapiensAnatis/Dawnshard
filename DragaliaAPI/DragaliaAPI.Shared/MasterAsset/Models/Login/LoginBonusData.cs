using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Login;

using MemoryPack;

[MemoryPackable]
public record LoginBonusData(
    int Id,
    bool IsLoop,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    EntityTypes EachDayEntityType,
    int EachDayEntityId,
    int EachDayEntityQuantity
);
