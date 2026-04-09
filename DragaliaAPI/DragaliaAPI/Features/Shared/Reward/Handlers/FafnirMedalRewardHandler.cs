using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

[UsedImplicitly]
public class FafnirMedalRewardHandler(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : IRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.FafnirMedal];

    public async Task<GrantReturn> Grant(Entity entity)
    {
        DbPlayerGatherItem gatherItem =
            await apiContext.PlayerGatherItems.FirstOrDefaultAsync(x => x.GatherItemId == entity.Id)
            ?? apiContext
                .PlayerGatherItems.Add(
                    new DbPlayerGatherItem
                    {
                        ViewerId = playerIdentityService.ViewerId,
                        GatherItemId = entity.Id,
                    }
                )
                .Entity;

        gatherItem.Quantity += entity.Quantity;

        return new(RewardGrantResult.Added);
    }
}
