using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Wall;

using MemoryPack;

[MemoryPackable]
public record QuestWallMonthlyReward(
    int Id,
    int TotalWallLevel,
    int RewardStageNum,
    string RewardImage,
    bool IsShowBadge,
    EntityTypes RewardEntityType,
    int RewardEntityId,
    int RewardEntityQuantity,
    string ReceiveStartDate,
    string ReceiveEndDate
) { }
