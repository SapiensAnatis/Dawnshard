using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

using MemoryPack;

[MemoryPackable]
public record MainStoryMissionGroupRewards(
    int Id,
    IEnumerable<MainStoryMissionGroupReward> Rewards
);

[MemoryPackable]
public record MainStoryMissionGroupReward(EntityTypes Type, int Id, int Quantity);
