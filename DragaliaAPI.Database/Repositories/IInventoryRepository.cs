using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IInventoryRepository
{
    DbPlayerCurrency AddCurrency(string deviceAccountId, CurrencyTypes type);
    IQueryable<DbPlayerCurrency> GetCurrencies(string deviceAccountId);
    Task<DbPlayerCurrency?> GetCurrency(string deviceAccountId, CurrencyTypes type);
    DbPlayerMaterial AddMaterial(string deviceAccountId, Materials type);
    Task<DbPlayerMaterial?> GetMaterial(string deviceAccountId, Materials materialId);
    IQueryable<DbPlayerMaterial> GetMaterials(string deviceAccountId);
    Task UpdateQuantity(IEnumerable<Materials> list, int quantity);
    Task UpdateQuantity(string deviceAccountId, Materials item, int quantity);
    Task<bool> CheckQuantity(IEnumerable<KeyValuePair<Materials, int>> quantityMap);
    Task<bool> CheckQuantity(Materials materialId, int quantity);
    Task UpdateQuantity(IEnumerable<KeyValuePair<Materials, int>> quantityMap);
    DbPlayerDragonGift AddDragonGift(string deviceAccountId, DragonGifts type);
    Task<DbPlayerDragonGift?> GetDragonGift(string deviceAccountId, DragonGifts materialId);
    IQueryable<DbPlayerDragonGift> GetDragonGifts(string deviceAccountId);
    Task RefreshPurchasableDragonGiftCounts(string deviceAccountId);
    Task UpdateQuantity(Materials item, int quantity);
}
