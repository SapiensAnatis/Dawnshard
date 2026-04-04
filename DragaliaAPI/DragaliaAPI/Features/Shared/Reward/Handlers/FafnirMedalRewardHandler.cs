using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

[UsedImplicitly]
public partial class FafnirMedalRewardHandler(ApiContext apiContext) : IRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.FafnirMedal];

    public async Task<GrantReturn> Grant(Entity entity)
    {
        DbPlayerGatherItem gatherItem =
            await apiContext.PlayerGatherItems.FirstOrDefaultAsync(x => x.GatherItemId == entity.Id)
            ?? apiContext
                .PlayerGatherItems.Add(new DbPlayerGatherItem { GatherItemId = entity.Id })
                .Entity;

        gatherItem.Quantity += entity.Quantity;

        return new(RewardGrantResult.Added);
    }
}
