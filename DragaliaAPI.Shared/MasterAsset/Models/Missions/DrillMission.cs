using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record DrillMission(
    int Id,
    string TextTitle,
    string TextDetail,
    string IconImage,
    int MissionDrillGroupId,
    int Step,
    int IsPickUp,
    int CompleteValue,
    int ProgressFlag,
    MissionTransportType MissionTransportType,
    int TransportValue,
    EntityTypes EntityType,
    int EntityId,
    int EntityQuantity,
    int NeedCompleteMissionId
) : IMission;
