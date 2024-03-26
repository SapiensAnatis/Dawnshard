using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

[MemoryPackable]
public partial record SpecialMission(
    int Id,
    string Text,
    int SortId,
    int MissionSpecialGroupId,
    int CompleteValue,
    int ProgressFlag,
    MissionTransportType MissionTransportType,
    int TransportValue,
    EntityTypes EntityType,
    int EntityId,
    int EntityQuantity
) : IMission;
