using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record NormalMission(
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
    int NeedCompleteMissionId,
    int NeedClearQuestId,
    int BlankViewQuestStoryId,
    int BlankViewQuestId
) : IMission;
