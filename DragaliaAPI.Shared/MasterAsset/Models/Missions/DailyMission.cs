using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record DailyMission(
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
    string LockReceiveRewardEndDate,
    MissionLockRewardType LockRewardType,
    int LockReceiveRewardParam,
    int IsPickup,
    int BlankViewQuestStoryId,
    int BlankViewQuestId
) : IExtendedRewardMission;
