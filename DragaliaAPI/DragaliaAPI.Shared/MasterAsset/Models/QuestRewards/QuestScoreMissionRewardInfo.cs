using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;

public record QuestScoreMissionRewardInfo(
    int Id,
    Materials RewardEntityId,
    IEnumerable<QuestMissionCondition> RewardConditions
);

public record QuestMissionCondition(
    QuestCompleteType QuestCompleteType,
    int QuestCompleteValue,
    int PercentageAdded
);
