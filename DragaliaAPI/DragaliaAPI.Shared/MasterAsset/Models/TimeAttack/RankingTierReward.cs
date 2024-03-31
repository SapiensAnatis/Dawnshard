using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.TimeAttack;

public record RankingTierReward(
    int Id,
    int GroupId,
    float ClearTimeLower,
    float ClearTimeUpper,
    int QuestId,
    EntityTypes RankingRewardEntityType,
    int RankingRewardEntityId,
    int RankingRewardEntityQuantity
);
