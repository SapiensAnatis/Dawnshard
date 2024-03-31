using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;

public record QuestRewardData(
    int Id,
    QuestCompleteType MissionCompleteType1,
    int MissionCompleteValues1,
    QuestCompleteType MissionCompleteType2,
    int MissionCompleteValues2,
    QuestCompleteType MissionCompleteType3,
    int MissionCompleteValues3,
    EntityTypes MissionsClearSetEntityType1,
    int MissionsClearSetEntityId1,
    int MissionsClearSetEntityQuantity1,
    EntityTypes MissionsClearSetEntityType2,
    int MissionsClearSetEntityId2,
    int MissionsClearSetEntityQuantity2,
    EntityTypes MissionsClearSetEntityType3,
    int MissionsClearSetEntityId3,
    int MissionsClearSetEntityQuantity3,
    EntityTypes MissionCompleteEntityType,
    int MissionCompleteEntityId,
    int MissionCompleteEntityQuantity,
    int QuestScoreMissionId,
    int QuestScoringEnemyGroupId,
    EntityTypes FirstClearSetEntityType1,
    int FirstClearSetEntityId1,
    int FirstClearSetEntityQuantity1,
    EntityTypes FirstClearSetEntityType2,
    int FirstClearSetEntityId2,
    int FirstClearSetEntityQuantity2,
    EntityTypes FirstClearSetEntityType3,
    int FirstClearSetEntityId3,
    int FirstClearSetEntityQuantity3,
    EntityTypes FirstClearSetEntityType4,
    int FirstClearSetEntityId4,
    int FirstClearSetEntityQuantity4,
    EntityTypes FirstClearSetEntityType5,
    int FirstClearSetEntityId5,
    int FirstClearSetEntityQuantity5
)
{
    [IgnoreMember]
    public readonly (QuestCompleteType Type, int Value)[] Missions =
    {
        (MissionCompleteType1, MissionCompleteValues1),
        (MissionCompleteType2, MissionCompleteValues2),
        (MissionCompleteType3, MissionCompleteValues3)
    };

    [IgnoreMember]
    public readonly (EntityTypes Type, int Id, int Quantity)[] Entities =
    {
        (MissionsClearSetEntityType1, MissionsClearSetEntityId1, MissionsClearSetEntityQuantity1),
        (MissionsClearSetEntityType2, MissionsClearSetEntityId2, MissionsClearSetEntityQuantity2),
        (MissionsClearSetEntityType3, MissionsClearSetEntityId3, MissionsClearSetEntityQuantity3)
    };

    [IgnoreMember]
    public readonly (EntityTypes Type, int Id, int Quantity)[] FirstClearEntities =
    {
        (FirstClearSetEntityType1, FirstClearSetEntityId1, FirstClearSetEntityQuantity1),
        (FirstClearSetEntityType2, FirstClearSetEntityId2, FirstClearSetEntityQuantity2),
        (FirstClearSetEntityType3, FirstClearSetEntityId3, FirstClearSetEntityQuantity3),
        (FirstClearSetEntityType4, FirstClearSetEntityId4, FirstClearSetEntityQuantity4),
        (FirstClearSetEntityType5, FirstClearSetEntityId5, FirstClearSetEntityQuantity5),
    };
};
