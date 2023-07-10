using System.Collections;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MaterialsEnum = DragaliaAPI.Shared.Definitions.Enums.Materials;
using DragonGiftsEnum = DragaliaAPI.Shared.Definitions.Enums.DragonGifts;

namespace DragaliaAPI.Database.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<InventoryRepository> logger;

    public InventoryRepository(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<InventoryRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public IQueryable<DbPlayerCurrency> Currencies =>
        this.apiContext.PlayerWallet.Where(
            wallet => wallet.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerMaterial> Materials =>
        this.apiContext.PlayerMaterials.Where(
            storage => storage.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerDragonGift> DragonGifts =>
        this.apiContext.PlayerDragonGifts.Where(
            gifts => gifts.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public DbPlayerCurrency AddCurrency(CurrencyTypes type)
    {
        return apiContext.PlayerWallet
            .Add(
                new DbPlayerCurrency()
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
                    CurrencyType = type,
                    Quantity = 0
                }
            )
            .Entity;
    }

    public async Task<DbPlayerCurrency?> GetCurrency(CurrencyTypes type)
    {
        return await this.apiContext.PlayerWallet.FindAsync(
            this.playerIdentityService.AccountId,
            type
        );
    }

    public DbPlayerMaterial AddMaterial(Materials type)
    {
        return apiContext.PlayerMaterials
            .Add(
                new DbPlayerMaterial()
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
                    MaterialId = type,
                    Quantity = 0
                }
            )
            .Entity;
    }

    public async Task UpdateQuantity(Materials item, int quantity)
    {
        if (item == MaterialsEnum.Empty)
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
                this.playerIdentityService.AccountId,
                item
            )
            ?? (
                await this.apiContext.AddAsync(
                    new DbPlayerMaterial()
                    {
                        DeviceAccountId = this.playerIdentityService.AccountId,
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

        this.logger.LogTrace(
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

        this.logger.LogTrace("Updated player materials by map {@map}", quantityMap);
    }

    public async Task<DbPlayerMaterial?> GetMaterial(Materials materialId)
    {
        return await this.apiContext.PlayerMaterials.FindAsync(
            this.playerIdentityService.AccountId,
            materialId
        );
    }

    public async Task<bool> CheckQuantity(Materials materialId, int quantity) =>
        await this.CheckQuantity(new Dictionary<Materials, int>() { { materialId, quantity } });

    public async Task<bool> CheckQuantity(IEnumerable<KeyValuePair<Materials, int>> quantityMap)
    {
        foreach (KeyValuePair<Materials, int> requested in quantityMap)
        {
            if (requested.Key == MaterialsEnum.Empty)
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

    public DbPlayerDragonGift AddDragonGift(DragonGifts giftId)
    {
        return apiContext.PlayerDragonGifts
            .Add(
                new DbPlayerDragonGift()
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
                    DragonGiftId = giftId,
                    Quantity = 0
                }
            )
            .Entity;
    }

    public async Task<DbPlayerDragonGift?> GetDragonGift(DragonGifts giftId)
    {
        return await this.apiContext.PlayerDragonGifts.FindAsync(
            this.playerIdentityService.AccountId,
            giftId
        );
    }

    public async Task RefreshPurchasableDragonGiftCounts()
    {
        Dictionary<DragonGifts, DbPlayerDragonGift> dbGifts = await DragonGifts.ToDictionaryAsync(
            x => x.DragonGiftId
        );
        foreach (
            DragonGifts gift in Enum.GetValues<DragonGifts>()
                .Where(x => x < DragonGiftsEnum.FourLeafClover)
        )
        {
            if (dbGifts.TryGetValue(gift, out DbPlayerDragonGift? dbGift))
            {
                dbGift.Quantity = 1;
            }
            else
            {
                dbGift = AddDragonGift(gift);
                dbGift.Quantity = 1;
            }
        }
    }
}
