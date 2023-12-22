using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DragonGiftsEnum = DragaliaAPI.Shared.Definitions.Enums.DragonGifts;
using MaterialsEnum = DragaliaAPI.Shared.Definitions.Enums.Materials;

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

    public IQueryable<DbPlayerMaterial> Materials =>
        this.apiContext.PlayerMaterials.Where(
            storage => storage.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbPlayerDragonGift> DragonGifts =>
        this.apiContext.PlayerDragonGifts.Where(
            gifts => gifts.ViewerId == this.playerIdentityService.ViewerId
        );

    public DbPlayerMaterial AddMaterial(Materials type)
    {
        return apiContext
            .PlayerMaterials.Add(
                new DbPlayerMaterial()
                {
                    ViewerId = this.playerIdentityService.ViewerId,
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
                this.playerIdentityService.ViewerId,
                item
            )
            ?? (
                await this.apiContext.AddAsync(
                    new DbPlayerMaterial()
                    {
                        ViewerId = this.playerIdentityService.ViewerId,
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
            this.playerIdentityService.ViewerId,
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

    public DbPlayerDragonGift AddDragonGift(DragonGifts giftId, int quantity) =>
        apiContext
            .PlayerDragonGifts.Add(
                new DbPlayerDragonGift()
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    DragonGiftId = giftId,
                    Quantity = quantity
                }
            )
            .Entity;

    public async Task<DbPlayerDragonGift?> GetDragonGift(DragonGifts giftId)
    {
        return await this.apiContext.PlayerDragonGifts.FindAsync(
            this.playerIdentityService.ViewerId,
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
                this.AddDragonGift(gift, 1);
            }
        }
    }
}
