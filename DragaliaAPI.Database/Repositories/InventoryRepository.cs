using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

// TODO: add tests
public class InventoryRepository : BaseRepository, IInventoryRepository
{
    private readonly ApiContext apiContext;

    public InventoryRepository(ApiContext apiContext) : base(apiContext)
    {
        this.apiContext = apiContext;
    }

    public DbPlayerCurrency AddCurrency(string deviceAccountId, CurrencyTypes type)
    {
        return apiContext.PlayerWallet
            .Add(
                new DbPlayerCurrency()
                {
                    DeviceAccountId = deviceAccountId,
                    CurrencyType = type,
                    Quantity = 0
                }
            )
            .Entity;
    }

    public async Task<DbPlayerCurrency?> GetCurrency(string deviceAccountId, CurrencyTypes type)
    {
        return await this.apiContext.PlayerWallet.FirstOrDefaultAsync(
            entry => entry.CurrencyType == type
        );
    }

    public IQueryable<DbPlayerCurrency> GetCurrencies(string deviceAccountId)
    {
        return this.apiContext.PlayerWallet.Where(
            wallet => wallet.DeviceAccountId == deviceAccountId
        );
    }

    public DbPlayerMaterial AddMaterial(string deviceAccountId, Materials type)
    {
        return apiContext.PlayerStorage
            .Add(
                new DbPlayerMaterial()
                {
                    DeviceAccountId = deviceAccountId,
                    MaterialId = type,
                    Quantity = 0
                }
            )
            .Entity;
    }

    public async Task AddMaterialQuantity(string deviceAccountId, Materials item, int quantity)
    {
        DbPlayerMaterial material =
            await this.apiContext.PlayerStorage.FindAsync(deviceAccountId, item)
            ?? (
                await this.apiContext.AddAsync(
                    new DbPlayerMaterial()
                    {
                        DeviceAccountId = deviceAccountId,
                        MaterialId = item,
                        Quantity = 0
                    }
                )
            ).Entity;

        if (material.Quantity + quantity < 0)
        {
            // TODO: move DragaliaException into shared
            throw new InvalidOperationException(
                $"Could not modify material {item} by quantity {quantity}: existing quantity {material.Quantity} was too low"
            );
        }

        material.Quantity += quantity;
    }

    public async Task AddMaterialQuantity(
        string deviceAccountId,
        IEnumerable<Materials> list,
        int quantity
    )
    {
        foreach (Materials m in list)
        {
            // Db query (find) in loop??? Any way to do this better???
            await this.AddMaterialQuantity(deviceAccountId, m, quantity);
        }
    }

    public async Task<DbPlayerMaterial?> GetMaterial(string deviceAccountId, Materials materialId)
    {
        return await this.apiContext.PlayerStorage.FirstOrDefaultAsync(
            entry => entry.MaterialId == materialId
        );
    }

    public IQueryable<DbPlayerMaterial> GetMaterials(string deviceAccountId)
    {
        return this.apiContext.PlayerStorage.Where(
            storage => storage.DeviceAccountId == deviceAccountId
        );
    }
}
