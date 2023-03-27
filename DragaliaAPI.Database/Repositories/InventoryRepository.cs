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
        return await this.apiContext.PlayerWallet.FindAsync(deviceAccountId, type);
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

    public async Task UpdateQuantity(Materials item, int quantity)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        await this.UpdateQuantity(this.playerDetailsService.AccountId, item, quantity);
#pragma warning restore CS0618 // Type or member is obsolete
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

    public async Task UpdateQuantity(IEnumerable<Materials> list, int quantity)
    {
        foreach (Materials m in list)
        {
            await this.UpdateQuantity(m, quantity);
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

    public DbPlayerDragonGift AddDragonGift(string deviceAccountId, DragonGifts giftId)
    {
        return apiContext.PlayerDragonGifts
            .Add(
                new DbPlayerDragonGift()
                {
                    DeviceAccountId = deviceAccountId,
                    DragonGiftId = giftId,
                    Quantity = 0
                }
            )
            .Entity;
    }

    public async Task<DbPlayerDragonGift?> GetDragonGift(string deviceAccountId, DragonGifts giftId)
    {
        return await this.apiContext.PlayerDragonGifts.FindAsync(deviceAccountId, giftId);
    }

    public IQueryable<DbPlayerDragonGift> GetDragonGifts(string deviceAccountId)
    {
        return this.apiContext.PlayerDragonGifts.Where(
            gifts => gifts.DeviceAccountId == deviceAccountId
        );
    }

    public async Task RefreshPurchasableDragonGiftCounts(string deviceAccountId)
    {
        Dictionary<DragonGifts, DbPlayerDragonGift> dbGifts = await GetDragonGifts(deviceAccountId)
            .ToDictionaryAsync(x => x.DragonGiftId);
        foreach (
            DragonGifts gift in Enum.GetValues<DragonGifts>()
                .Where(x => x < DragonGifts.FourLeafClover)
        )
        {
            if (dbGifts.TryGetValue(gift, out DbPlayerDragonGift? dbGift))
            {
                dbGift.Quantity = 1;
            }
            else
            {
                dbGift = AddDragonGift(deviceAccountId, gift);
                dbGift.Quantity = 1;
            }
        }
    }
}
