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
}
