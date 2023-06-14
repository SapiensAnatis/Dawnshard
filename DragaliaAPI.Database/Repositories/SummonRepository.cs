using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class SummonRepository : BaseRepository, ISummonRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public SummonRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbPlayerSummonHistory> SummonHistory =>
        this.apiContext.PlayerSummonHistory.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task<DbPlayerBannerData> GetPlayerBannerData(int bannerId)
    {
        DbPlayerBannerData bannerData =
            await apiContext.PlayerBannerData.FirstOrDefaultAsync(
                x =>
                    x.DeviceAccountId.Equals(this.playerIdentityService.AccountId)
                    && x.SummonBannerId == bannerId
            ) ?? await this.AddPlayerBannerData(bannerId);
        return bannerData;
    }

    public async Task<DbPlayerBannerData> AddPlayerBannerData(int bannerId)
    {
        DbPlayerBannerData bannerData = DbPlayerBannerDataFactory.Create(
            this.playerIdentityService.AccountId,
            bannerId
        );
        bannerData = (await apiContext.PlayerBannerData.AddAsync(bannerData)).Entity;

        return bannerData;
    }

    public async Task AddSummonHistory(DbPlayerSummonHistory summonHistory)
    {
        await apiContext.PlayerSummonHistory.AddRangeAsync(summonHistory);
    }
}
