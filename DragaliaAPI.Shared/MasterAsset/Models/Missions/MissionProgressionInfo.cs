using System.Text.Json.Serialization;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record MissionInfo(
    int Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionType Type
);

public record MissionProgressionRequirement(
    IEnumerable<MissionInfo> Missions,
    int? Parameter = null,
    int? Parameter2 = null,
    int? Parameter3 = null,
    int? Parameter4 = null
);

public record MissionProgressionInfo(
    [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionProgressType Type,
    IEnumerable<MissionProgressionRequirement> Requirements
);
