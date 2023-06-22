using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Reward;

public interface IRewardService
{
    /// <summary>
    /// Grant a reward of an arbitrary entity type to the player.
    /// </summary>
    /// <param name="entity">The entity to grant.</param>
    /// <returns>
    /// An enum indicating the result of the add operation.
    /// </returns>
    Task<RewardGrantResult> GrantReward(Entity entity);

    EntityResult GetEntityResult();
}
