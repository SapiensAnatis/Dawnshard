using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;

using MemoryPack;

[MemoryPackable]
public record QuestScoreMissionRewardInfo(
    int Id,
    Materials RewardEntityId,
    IEnumerable<QuestMissionCondition> RewardConditions
);

[MemoryPackable]
public record QuestMissionCondition(
    QuestCompleteType QuestCompleteType,
    int QuestCompleteValue,
    int PercentageAdded
);
