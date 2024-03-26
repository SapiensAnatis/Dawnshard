using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

[MemoryPackable]
public partial record MainStoryMission(
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
