using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

[MemoryPackable]
public partial record MainStoryMissionGroupRewards(
    int Id,
    IEnumerable<MainStoryMissionGroupReward> Rewards
);

[MemoryPackable]
public partial record MainStoryMissionGroupReward(EntityTypes Type, int Id, int Quantity);
