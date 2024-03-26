using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Login;

[MemoryPackable]
public partial record LoginBonusReward(
    int Id,
    int Gid,
    int Day,
    EntityTypes EntityType,
    int EntityId,
    int EntityQuantity,
    int EntityLevel,
    int EntityLimitBreakCount,
    int EntityBuildupCount,
    int EntityEquipableCount
);
