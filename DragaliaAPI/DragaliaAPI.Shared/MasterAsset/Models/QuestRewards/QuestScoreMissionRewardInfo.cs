using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;

[MemoryPackable]
public partial record QuestScoreMissionRewardInfo(
    int Id,
    Materials RewardEntityId,
    IEnumerable<QuestMissionCondition> RewardConditions
);

[MemoryPackable]
public partial record QuestMissionCondition(
    QuestCompleteType QuestCompleteType,
    int QuestCompleteValue,
    int PercentageAdded
);
