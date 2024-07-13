using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Summoning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Login.Actions;

public class DailySummoningDealResetAction(
    ApiContext apiContext,
    IOptionsMonitor<SummonBannerOptions> bannerOptionsMonitor,
    TimeProvider timeProvider
) : IDailyResetAction
{
    private readonly SummonBannerOptions bannerOptions = bannerOptionsMonitor.CurrentValue;

    public async Task Apply()
    {
        IEnumerable<int> activeBannerIds = this
            .bannerOptions.Banners.Where(x => x.GetIsCurrentlyActive())
            .Select(x => x.Id);

        List<DbPlayerBannerData> affectedBannerData = await apiContext
            .PlayerBannerData.Where(x =>
                activeBannerIds.Contains(x.SummonBannerId) && x.DailyLimitedSummonCount > 0
            )
            .AsTracking()
            .ToListAsync();

        foreach (DbPlayerBannerData banner in affectedBannerData)
        {
            banner.DailyLimitedSummonCount = 0;
        }
    }
}
