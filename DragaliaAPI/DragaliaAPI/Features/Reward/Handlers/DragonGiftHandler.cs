using System.Collections.Immutable;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward.Handlers;

[UsedImplicitly]
public class DragonGiftHandler(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    : IRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(EntityTypes.DragonGift);

    private Dictionary<DragonGifts, DbPlayerDragonGift>? dragonGiftCache;

    public async Task<GrantReturn> Grant(Entity reward)
    {
        DragonGifts gift = (DragonGifts)reward.Id;
        if (!Enum.IsDefined(gift))
            throw new ArgumentException($"Invalid dragon gift ID {reward.Id}", nameof(reward));

        this.dragonGiftCache ??= await apiContext
            .PlayerDragonGifts.AsTracking()
            .ToDictionaryAsync(x => x.DragonGiftId, x => x);

        if (this.dragonGiftCache.TryGetValue(gift, out DbPlayerDragonGift? dbEntity))
        {
            dbEntity.Quantity += reward.Quantity;
        }
        else
        {
            DbPlayerDragonGift dragonGift =
                new()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    DragonGiftId = gift,
                    Quantity = reward.Quantity
                };

            apiContext.PlayerDragonGifts.Add(dragonGift);
            this.dragonGiftCache[gift] = dragonGift;
        }

        return new GrantReturn(RewardGrantResult.Added);
    }
}
