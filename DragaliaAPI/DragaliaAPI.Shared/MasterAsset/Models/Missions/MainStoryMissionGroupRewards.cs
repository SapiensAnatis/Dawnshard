using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record MainStoryMissionGroupRewards(
    int Id,
    IEnumerable<MainStoryMissionGroupReward> Rewards
);

public record MainStoryMissionGroupReward(EntityTypes Type, int Id, int Quantity);
