using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Features.Item;

public interface IItemService
{
    Task<IEnumerable<ItemList>> GetItemList();
    Task<AtgenRecoverData> UseItems(IEnumerable<AtgenUseItemList> items);
}
