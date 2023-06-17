using System.Text.Json.Serialization;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record MissionInfo(
    int Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionType Type
);

public record MissionProgressionRequirement(
    IEnumerable<MissionInfo> Missions,
    int Parameter,
    int Parameter2 = -1,
    int Parameter3 = -1,
    int Parameter4 = -1
);

public record MissionProgressionInfo(
    [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionProgressType Type,
    IEnumerable<MissionProgressionRequirement> Requirements
);
