using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IInventoryRepository : IBaseRepository
{
    Task<DbPlayerCurrency> GetOrAddCurrency(string deviceAccountId, CurrencyTypes type);
    IQueryable<DbPlayerCurrency> GetCurrencies(string deviceAccountId);
    Task<DbPlayerCurrency?> GetCurrency(string deviceAccountId, CurrencyTypes type);
    Task<DbPlayerMaterial> GetOrAddMaterial(string deviceAccountId, Materials type);
    Task<DbPlayerMaterial?> GetMaterial(string deviceAccountId, Materials materialId);
    IQueryable<DbPlayerMaterial> GetMaterials(string deviceAccountId);
}
