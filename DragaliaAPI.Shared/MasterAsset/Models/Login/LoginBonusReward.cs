using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Login;

public record LoginBonusReward(
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
