using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Missions;

public interface IRewardService
{
    Task GrantReward(
        EntityTypes type,
        int id,
        int quantity = 1,
        int? limitBreakCount = null,
        int? buildupCount = null,
        int? equipableCount = null
    );
}
