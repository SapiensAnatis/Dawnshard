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
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task<DbPlayerShopInfo> GetShopInfoAsync()
    {
        return await ShopInfos.SingleAsync();
    }

    public void InitializeShopInfo()
    {
        this.apiContext.PlayerShopInfos.Add(
            new DbPlayerShopInfo() { DeviceAccountId = this.playerIdentityService.AccountId, }
        );
    }
}
