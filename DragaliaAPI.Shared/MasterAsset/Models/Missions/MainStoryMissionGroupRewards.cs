using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record MainStoryMissionGroupRewards(
    int Id,
    IEnumerable<MainStoryMissionGroupReward> Rewards
);

public record MainStoryMissionGroupReward(
    [property: JsonConverter(typeof(JsonStringEnumConverter))] EntityTypes Type,
    int Id,
    int Quantity
);
