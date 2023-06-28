using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IInventoryRepository
{
    IQueryable<DbPlayerCurrency> Currencies { get; }
    IQueryable<DbPlayerMaterial> Materials { get; }
    IQueryable<DbPlayerDragonGift> DragonGifts { get; }

    DbPlayerCurrency AddCurrency(CurrencyTypes type);
    Task<DbPlayerCurrency?> GetCurrency(CurrencyTypes type);
    DbPlayerMaterial AddMaterial(Materials type);
    Task<DbPlayerMaterial?> GetMaterial(Materials materialId);
    Task UpdateQuantity(IEnumerable<Materials> list, int quantity);
    Task UpdateQuantity(Materials item, int quantity);
    Task<bool> CheckQuantity(IEnumerable<KeyValuePair<Materials, int>> quantityMap);
    Task<bool> CheckQuantity(Materials materialId, int quantity);
    Task UpdateQuantity(IEnumerable<KeyValuePair<Materials, int>> quantityMap);
    DbPlayerDragonGift AddDragonGift(DragonGifts type);
    Task<DbPlayerDragonGift?> GetDragonGift(DragonGifts materialId);
    Task RefreshPurchasableDragonGiftCounts();
}
