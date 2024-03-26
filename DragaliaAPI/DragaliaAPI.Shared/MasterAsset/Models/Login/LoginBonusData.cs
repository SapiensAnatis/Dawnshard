using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Login;

[MemoryPackable]
public partial record LoginBonusData(
    int Id,
    bool IsLoop,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    EntityTypes EachDayEntityType,
    int EachDayEntityId,
    int EachDayEntityQuantity
);
