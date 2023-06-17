using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record PeriodMission(
    int Id,
    string Text,
    int SortId,
    int CompleteValue,
    int ProgressFlag,
    MissionTransportType MissionTransportType,
    int TransportValue,
    EntityTypes EntityType,
    int EntityId,
    int EntityQuantity,
    int EntityBuildupCount,
    int EntityLimitBreakCount,
    int EntityEquipableCount,
    int QuestGroupId,
    int CampaignId,
    int NeedCompleteMissionId,
    int NeedClearQuestId,
    int BlankViewQuestStoryId,
    int BlankViewQuestId
) : IExtendedRewardMission;
