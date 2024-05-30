using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using JetBrains.Annotations;

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
    [MustUseReturnValue]
    Task<RewardGrantResult> GrantReward(Entity entity);

    /// <summary>
    /// Grant a list of rewards to the player.
    /// </summary>
    /// <param name="entities">The rewards to grant.</param>
    /// <returns>The task.</returns>
    Task GrantRewards(IEnumerable<Entity> entities);

    [MustUseReturnValue]
    Task<(RewardGrantResult Result, DbTalisman? Talisman)> GrantTalisman(
        Talismans id,
        int abilityId1,
        int abilityId2,
        int abilityId3,
        int hp,
        int atk
    );

    EntityResult GetEntityResult();

    IEnumerable<ConvertedEntity> GetConvertedEntityList();

    Task<IDictionary<TKey, RewardGrantResult>> BatchGrantRewards<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct;
}
