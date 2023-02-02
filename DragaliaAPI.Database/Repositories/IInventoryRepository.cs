using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IInventoryRepository : IBaseRepository
{
    DbPlayerCurrency AddCurrency(string deviceAccountId, CurrencyTypes type);
    IQueryable<DbPlayerCurrency> GetCurrencies(string deviceAccountId);
    Task<DbPlayerCurrency?> GetCurrency(string deviceAccountId, CurrencyTypes type);
    DbPlayerMaterial AddMaterial(string deviceAccountId, Materials type);
    Task<DbPlayerMaterial?> GetMaterial(string deviceAccountId, Materials materialId);
    IQueryable<DbPlayerMaterial> GetMaterials(string deviceAccountId);
    Task AddMaterialQuantity(string deviceAccountId, IEnumerable<Materials> list, int quantity);
    Task AddMaterialQuantity(string deviceAccountId, Materials item, int quantity);
    
    /// <summary>
    /// Check that a player has at least a certain quantity of a material.
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="materialId"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    Task<bool> CheckHasMaterialQuantity(string accountId, Materials materialId, int quantity);

    Task<bool> CheckHasMaterialQuantity(
        string accountId,
        IEnumerable<KeyValuePair<Materials, int>> quantityMap
    );
}
