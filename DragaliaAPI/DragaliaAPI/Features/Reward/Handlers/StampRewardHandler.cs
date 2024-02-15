using System.Collections.Immutable;
using DragaliaAPI.Features.Stamp;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using JetBrains.Annotations;

namespace DragaliaAPI.Features.Reward.Handlers;

[UsedImplicitly]
public class StampRewardHandler : IRewardHandler
{
    public ImmutableArray<EntityTypes> SupportedTypes { get; } = [EntityTypes.Stamp];

    public Task<GrantReturn> Grant(Entity entity)
    {
        // Currently players have access to all stamps by default, so any new stamp
        // is guaranteed to be a duplicate.
        // Additionally, all stamps grant 25 wyrmite as the duplicate reward, so
        // there is no need to do a lookup here.
        return Task.FromResult(
            new GrantReturn(
                RewardGrantResult.Converted,
                new Entity(EntityTypes.Wyrmite, Quantity: 25)
            )
        );
    }
}
