using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Item;

public interface IItemRepository
{
    IQueryable<DbPlayerUseItem> Items { get; }

    Task AddItemQuantityAsync(UseItem item, int quantity);

    Task<DbPlayerUseItem?> GetItemAsync(UseItem id);
}
