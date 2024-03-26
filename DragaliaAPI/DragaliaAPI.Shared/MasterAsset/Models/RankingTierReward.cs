using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record RankingTierReward(
    int Id,
    int GroupId,
    float ClearTimeLower,
    float ClearTimeUpper,
    int QuestId,
    EntityTypes RankingRewardEntityType,
    int RankingRewardEntityId,
    int RankingRewardEntityQuantity
);
