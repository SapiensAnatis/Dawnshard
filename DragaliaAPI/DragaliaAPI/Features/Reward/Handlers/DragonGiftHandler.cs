using System.Collections.Immutable;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Reward.Handlers;

public class DragonGiftHandler(IInventoryRepository inventoryRepository) : IRewardHandler
{
    public ImmutableArray<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(EntityTypes.DragonGift);

    public async Task<GrantReturn> Grant(Entity reward)
    {
        DragonGifts gift = (DragonGifts)reward.Id;
        if (!Enum.IsDefined(gift))
            throw new ArgumentException($"Invalid dragon gift ID {reward.Id}", nameof(reward));

        DbPlayerDragonGift? dbEntity = await inventoryRepository.GetDragonGift(gift);

        if (dbEntity is null)
            inventoryRepository.AddDragonGift(gift, reward.Quantity);
        else
            dbEntity.Quantity += reward.Quantity;

        return new GrantReturn(RewardGrantResult.Added);
    }
}
