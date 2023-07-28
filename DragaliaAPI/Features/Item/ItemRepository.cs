using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.Item;

public class ItemRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    : IItemRepository
{
    public IQueryable<DbPlayerUseItem> Items =>
        apiContext.PlayerUseItems.Where(x => x.DeviceAccountId == playerIdentityService.AccountId);

    private async Task<DbPlayerUseItem?> GetItem(UseItem item)
    {
        return await apiContext.PlayerUseItems.FindAsync(playerIdentityService.AccountId, item);
    }

    public async Task AddItemQuantityAsync(UseItem id, int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        DbPlayerUseItem item =
            await GetItem(id)
            ?? apiContext.PlayerUseItems
                .Add(
                    new DbPlayerUseItem
                    {
                        DeviceAccountId = playerIdentityService.AccountId,
                        ItemId = id
                    }
                )
                .Entity;

        item.Quantity += quantity;
    }

    public async Task<DbPlayerUseItem?> GetItemAsync(UseItem id)
    {
        return await apiContext.PlayerUseItems.FindAsync(playerIdentityService.AccountId, id);
    }
}
