using DragaliaAPI.Shared.Definitions.Enums;

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
    int MissionCompleteEntityQuantity
)
{
    public readonly (QuestCompleteType Type, int Value)[] Missions =
    {
        (MissionCompleteType1, MissionCompleteValues1),
        (MissionCompleteType2, MissionCompleteValues2),
        (MissionCompleteType3, MissionCompleteValues3)
    };

    public readonly (EntityTypes Type, int Id, int Quantity)[] Entities =
    {
        (MissionsClearSetEntityType1, MissionsClearSetEntityId1, MissionsClearSetEntityQuantity1),
        (MissionsClearSetEntityType2, MissionsClearSetEntityId2, MissionsClearSetEntityQuantity2),
        (MissionsClearSetEntityType3, MissionsClearSetEntityId3, MissionsClearSetEntityQuantity3)
    };
};
