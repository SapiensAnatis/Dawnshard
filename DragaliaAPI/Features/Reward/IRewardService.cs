using DragaliaAPI.Database.Entities;
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

    Task<(RewardGrantResult Result, DbTalisman? Talisman)> GrantTalisman(
        Talismans id,
        int abilityId1,
        int abilityId2,
        int abilityId3,
        int hp,
        int atk
    );

    EntityResult GetEntityResult();
}
