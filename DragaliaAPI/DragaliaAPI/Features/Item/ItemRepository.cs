using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.Item;

public class ItemRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    : IItemRepository
{
    public IQueryable<DbPlayerUseItem> Items =>
        apiContext.PlayerUseItems.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    private async Task<DbPlayerUseItem?> GetItem(UseItem item)
    {
        return await apiContext.PlayerUseItems.FindAsync(playerIdentityService.ViewerId, item);
    }

    public async Task AddItemQuantityAsync(UseItem id, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);

        DbPlayerUseItem item =
            await GetItem(id)
            ?? apiContext
                .PlayerUseItems.Add(
                    new DbPlayerUseItem { ViewerId = playerIdentityService.ViewerId, ItemId = id }
                )
                .Entity;

        item.Quantity += quantity;
    }

    public async Task<DbPlayerUseItem?> GetItemAsync(UseItem id)
    {
        return await apiContext.PlayerUseItems.FindAsync(playerIdentityService.ViewerId, id);
    }
}
