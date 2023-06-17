using System.Text.Json.Serialization;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record MissionInfo(
    int Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionType Type
);

public record MissionProgressionRequirement(
    int Parameter,
    int Parameter2,
    int Parameter3,
    IEnumerable<MissionInfo> Missions
);

public record MissionProgressionInfo(
    [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionProgressType Type,
    IEnumerable<MissionProgressionRequirement> Requirements
);
