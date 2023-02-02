using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

// TODO: add tests
public class InventoryRepository : BaseRepository, IInventoryRepository
{
    private readonly ApiContext apiContext;
    private readonly ILogger<InventoryRepository> logger;

    public InventoryRepository(ApiContext apiContext, ILogger<InventoryRepository> logger)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.logger = logger;
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
        return apiContext.PlayerMaterials
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
        if (item == Materials.Empty)
            return;

        DbPlayerMaterial material =
            await this.apiContext.PlayerMaterials.FindAsync(deviceAccountId, item)
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
        return await this.apiContext.PlayerMaterials.FindAsync(deviceAccountId, materialId);
    }

    public IQueryable<DbPlayerMaterial> GetMaterials(string deviceAccountId)
    {
        return this.apiContext.PlayerMaterials.Where(
            storage => storage.DeviceAccountId == deviceAccountId
        );
    }

    public async Task<bool> CheckHasMaterialQuantity(
        string accountId,
        Materials materialId,
        int quantity
    )
    {
        if (materialId == Materials.Empty)
            return true;

        DbPlayerMaterial? material = await this.GetMaterial(accountId, materialId);
        bool result = (material)?.Quantity >= quantity;

        if (!result)
            this.logger.LogDebug(
                "Failed material check for material {@material}: needed quantity {quantity}",
                material,
                quantity
            );

        return result;
    }

    public async Task<bool> CheckHasMaterialQuantity(
        string accountId,
        IEnumerable<KeyValuePair<Materials, int>> quantityMap
    )
    {
        IEnumerable<Materials> materials = quantityMap.Select(x => x.Key);

        Dictionary<Materials, DbPlayerMaterial> dbValues = await this.apiContext.PlayerMaterials
            .Where(x => materials.Contains(x.MaterialId))
            .ToDictionaryAsync(x => x.MaterialId, x => x);

        foreach (KeyValuePair<Materials, int> requested in quantityMap)
        {
            if (requested.Key == Materials.Empty)
                continue;

            if (!dbValues.TryGetValue(requested.Key, out DbPlayerMaterial? mat))
                mat = new() { DeviceAccountId = accountId, Quantity = 0 };

            if (mat.Quantity < requested.Value)
            {
                this.logger.LogDebug(
                    "Played failed material {material} check: requested quantity {q1}, owned: {mat}",
                    requested.Key,
                    requested.Value,
                    mat
                );

                return false;
            }
        }

        return true;
    }
}
