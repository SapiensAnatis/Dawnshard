using System.Text.Json.Serialization;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

[MemoryPackable]
public partial record MissionProgressionInfo(
    int Id,
    MissionType MissionType,
    int MissionId,
    MissionCompleteType CompleteType,
    bool UseTotalValue,
    int? ProgressionGroupId = null,
    int[]? UnlockedOnComplete = null,
    int? Parameter = null,
    int? Parameter2 = null,
    int? Parameter3 = null,
    int? Parameter4 = null
);
