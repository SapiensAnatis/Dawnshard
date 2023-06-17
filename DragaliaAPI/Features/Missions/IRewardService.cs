using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Missions;

public interface IRewardService
{
    Task GrantReward(
        EntityTypes type,
        int id,
        int quantity = 1,
        int limitBreakCount = -1,
        int buildupCount = -1,
        int equipableCount = -1
    );
}
