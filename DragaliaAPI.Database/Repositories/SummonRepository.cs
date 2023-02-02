using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class SummonRepository : BaseRepository, ISummonRepository
{
    private readonly ApiContext apiContext;

    public SummonRepository(ApiContext apiContext)
        : base(apiContext)
    {
        this.apiContext = apiContext;
    }

    public async Task<List<DbPlayerSummonHistory>> GetSummonHistory(string deviceAccountId)
    {
        return await apiContext.PlayerSummonHistory
            .Where(x => x.DeviceAccountId.Equals(deviceAccountId))
            .ToListAsync();
    }

    public async Task<DbPlayerBannerData> GetPlayerBannerData(string deviceAccountId, int bannerId)
    {
        DbPlayerBannerData bannerData =
            await apiContext.PlayerBannerData.FirstOrDefaultAsync(
                x => x.DeviceAccountId.Equals(deviceAccountId) && x.SummonBannerId == bannerId
            ) ?? await this.AddPlayerBannerData(deviceAccountId, bannerId);
        return bannerData;
    }

    public async Task<DbPlayerBannerData> AddPlayerBannerData(string deviceAccountId, int bannerId)
    {
        DbPlayerBannerData bannerData = DbPlayerBannerDataFactory.Create(deviceAccountId, bannerId);
        bannerData = (await apiContext.PlayerBannerData.AddAsync(bannerData)).Entity;

        return bannerData;
    }

    public async Task AddSummonHistory(IEnumerable<DbPlayerSummonHistory> summonHistory)
    {
        await apiContext.PlayerSummonHistory.AddRangeAsync(summonHistory);
    }
}
