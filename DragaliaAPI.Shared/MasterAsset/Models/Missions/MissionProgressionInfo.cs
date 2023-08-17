using System.Text.Json.Serialization;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record MissionProgressionInfo(
    int Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionType MissionType,
    int MissionId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionCompleteType CompleteType,
    bool UseTotalValue,
    int? Parameter = null,
    int? Parameter2 = null,
    int? Parameter3 = null,
    int? Parameter4 = null
);
