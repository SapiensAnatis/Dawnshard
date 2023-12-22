using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shop;

public class ShopRepository : IShopRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public ShopRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbPlayerShopInfo> ShopInfos =>
        this.apiContext.PlayerShopInfos.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbPlayerShopPurchase> Purchases =>
        this.apiContext.PlayerPurchases.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public async Task<DbPlayerShopInfo> GetShopInfoAsync()
    {
        return await this.apiContext.PlayerShopInfos.FindAsync(this.playerIdentityService.ViewerId)
            ?? throw new NullReferenceException("No ShopInfo found");
    }

    public void InitializeShopInfo()
    {
        this.apiContext.PlayerShopInfos.Add(
            new DbPlayerShopInfo() { ViewerId = this.playerIdentityService.ViewerId, }
        );
    }

    public async Task<int> GetDailySummonCountAsync()
    {
        return await this.ShopInfos.Select(x => x.DailySummonCount).SingleAsync();
    }

    public async Task ClearExpiredShopPurchases()
    {
        DateTimeOffset current = DateTimeOffset.UtcNow;

        await this.Purchases.Where(
            x => x.EffectEndTime != DateTimeOffset.UnixEpoch && current >= x.EffectEndTime
        )
            .ExecuteDeleteAsync();
    }

    public async Task<bool> AddShopPurchase(
        ShopType type,
        int goodsId,
        int quantity,
        DateTimeOffset buyTime,
        DateTimeOffset effectStart,
        DateTimeOffset effectEnd
    )
    {
        await ClearExpiredShopPurchases();

        DbPlayerShopPurchase? existing = await this.Purchases.FirstOrDefaultAsync(
            x => x.GoodsId == goodsId
        );
        if (existing == null)
        {
            this.apiContext.PlayerPurchases.Add(
                new DbPlayerShopPurchase()
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    ShopType = type.ToPurchaseShopType(),
                    GoodsId = goodsId,
                    BuyCount = quantity,
                    LastBuyTime = buyTime,
                    EffectStartTime = effectStart,
                    EffectEndTime = effectEnd
                }
            );

            return true;
        }
        else
        {
            existing.BuyCount += quantity;
            existing.LastBuyTime = buyTime;
        }

        return false;
    }
}
