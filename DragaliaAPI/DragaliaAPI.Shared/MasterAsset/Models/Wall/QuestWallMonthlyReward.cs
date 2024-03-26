using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Wall;

[MemoryPackable]
public partial record QuestWallMonthlyReward(
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
