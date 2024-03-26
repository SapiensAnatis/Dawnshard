using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record MainStoryMission(
    int Id,
    string Text,
    int MissionMainStoryGroupId,
    int SortId,
    int CompleteValue,
    int ProgressFlag,
    MissionTransportType MissionTransportType,
    int TransportValue,
    EntityTypes EntityType,
    int EntityId,
    int EntityQuantity,
    int NeedCompleteMissionId,
    int BlankViewQuestStoryId,
    int BlankViewQuestId
) : IMission;
