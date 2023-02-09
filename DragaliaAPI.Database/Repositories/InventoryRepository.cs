using System.Collections;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;
    private readonly ILogger<InventoryRepository> logger;

    public InventoryRepository(
        ApiContext apiContext,
        IPlayerDetailsService playerDetailsService,
        ILogger<InventoryRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
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

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public async Task UpdateQuantity(string deviceAccountId, Materials item, int quantity)
    {
        if (item == Materials.Empty)
            return;

        DbPlayerMaterial material = await this.FindAsync(item);

        if (material.Quantity + quantity < 0)
        {
            // TODO: move DragaliaException into shared
            throw new InvalidOperationException(
                $"Could not modify material {item} by quantity {quantity}: existing quantity {material.Quantity} was too low"
            );
        }

        material.Quantity += quantity;
    }

    private async Task<DbPlayerMaterial> FindAsync(Materials item)
    {
        return await this.apiContext.PlayerMaterials.FindAsync(
                this.playerDetailsService.AccountId,
                item
            )
            ?? (
                await this.apiContext.AddAsync(
                    new DbPlayerMaterial()
                    {
                        DeviceAccountId = this.playerDetailsService.AccountId,
                        MaterialId = item,
                        Quantity = 0
                    }
                )
            ).Entity;
    }

    public async Task UpdateQuantity(Materials material, int quantity)
    {
#pragma warning disable CS0618
        await this.UpdateQuantity(this.playerDetailsService.AccountId, material, quantity);
#pragma warning restore CS0618
    }

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public async Task UpdateQuantity(
        string deviceAccountId,
        IEnumerable<Materials> list,
        int quantity
    )
    {
        foreach (Materials m in list)
        {
            await this.UpdateQuantity(deviceAccountId, m, quantity);
        }

        this.logger.LogDebug(
            "Updated list of materials by quantity {quantity}: {list}",
            quantity,
            list
        );
    }

    public async Task UpdateQuantity(IEnumerable<KeyValuePair<Materials, int>> quantityMap)
    {
        foreach ((Materials mat, int quantity) in quantityMap)
        {
            await this.UpdateQuantity(mat, quantity);
        }

        this.logger.LogDebug("Updated player materials by map {@map}", quantityMap);
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

    public async Task<bool> CheckQuantity(Materials materialId, int quantity) =>
        await this.CheckQuantity(new Dictionary<Materials, int>() { { materialId, quantity } });

    public async Task<bool> CheckQuantity(IEnumerable<KeyValuePair<Materials, int>> quantityMap)
    {
        foreach (KeyValuePair<Materials, int> requested in quantityMap)
        {
            if (requested.Key == Materials.Empty)
                continue;

            DbPlayerMaterial mat = await this.FindAsync(requested.Key);

            if (mat?.Quantity < requested.Value)
            {
                this.logger.LogWarning(
                    "Failed material {material} check: requested quantity {q1}, entity: {@mat}",
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
