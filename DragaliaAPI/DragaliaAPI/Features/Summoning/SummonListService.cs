using DragaliaAPI.Database;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Summoning;

/// <summary>
/// Service to provide data for /summon/get_summon_list, which returns an overview of available banners.
/// </summary>
public sealed class SummonListService(
    IOptionsMonitor<SummonBannerOptions> optionsMonitor,
    IPlayerIdentityService playerIdentityService,
    ApiContext apiContext
)
{
    private const int SingleCrystalCost = 120;
    private const int SingleDiamondCost = 120;

    private const int MultiCrystalCost = 1200;
    private const int MultiDiamondCost = 1200;

    private const int LimitedCrystalCost = 0;
    private const int LimitedDiamondCost = 30;

    private const int AddSummonPoint = 1;
    private const int AddSummonPointStone = 2;
    private const int ExchangeSummonPoint = 300;

    private const int DailyLimit = 1;

    public async Task<List<SummonList>> GetSummonList()
    {
        List<SummonList> results = new(optionsMonitor.CurrentValue.Banners.Count);

        var individualDataDict = await apiContext
            .PlayerBannerData.Where(x => x.ViewerId == playerIdentityService.ViewerId)
            .Select(x => new
            {
                BannerId = x.SummonBannerId,
                DailyCount = x.DailyLimitedSummonCount,
                TotalCount = x.SummonCount
            })
            .ToDictionaryAsync(x => x.BannerId, x => x);

        foreach (Banner banner in optionsMonitor.CurrentValue.Banners)
        {
            if (banner.Id == SummonConstants.RedoableSummonBannerId)
                continue;

            int dailyCount = 0;
            int totalCount = 0;

            if (individualDataDict.TryGetValue(banner.Id, out var bannerData))
            {
                dailyCount = bannerData.DailyCount;
                totalCount = bannerData.TotalCount;
            }

            results.Add(
                new SummonList()
                {
                    SummonId = banner.Id,
                    SummonType = 2, // No idea what this does.
                    SingleCrystal = SingleCrystalCost,
                    SingleDiamond = SingleDiamondCost,
                    MultiCrystal = MultiCrystalCost,
                    MultiDiamond = MultiDiamondCost,
                    LimitedCrystal = LimitedCrystalCost,
                    LimitedDiamond = LimitedDiamondCost,
                    SummonPointId = banner.Id,
                    AddSummonPoint = AddSummonPoint,
                    AddSummonPointStone = AddSummonPointStone,
                    ExchangeSummonPoint = ExchangeSummonPoint,
                    Status = 1,
                    CommenceDate = banner.Start,
                    CompleteDate = banner.End,
                    DailyCount = dailyCount,
                    DailyLimit = DailyLimit,
                    TotalCount = totalCount,
                    TotalLimit = 0,
                }
            );
        }

        return results;
    }

    public async Task<SummonList?> GetSummonList(int bannerId)
    {
        // Note: could be written to only fetch player data for a single banner to be more efficient. Should still be
        // one query so not a high priority for now.
        if (bannerId == SummonConstants.RedoableSummonBannerId)
            return null;

        List<SummonList> availableBanners = await GetSummonList();
        return availableBanners.Find(x => x.SummonId == bannerId);
    }
}
