using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Features.Item;

public interface IItemRepository
{
    IQueryable<DbPlayerUseItem> Items { get; }

    Task AddItemQuantityAsync(UseItem item, int quantity);

    Task<DbPlayerUseItem?> GetItemAsync(UseItem id);
}
