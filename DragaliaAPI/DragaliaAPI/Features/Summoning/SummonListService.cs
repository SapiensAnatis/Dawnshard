using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
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

    public async Task<IEnumerable<SummonList>> GetSummonList()
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
                    summon_id = banner.Id,
                    summon_type = 2, // No idea what this does.
                    single_crystal = SingleCrystalCost,
                    single_diamond = SingleDiamondCost,
                    multi_crystal = MultiCrystalCost,
                    multi_diamond = MultiDiamondCost,
                    limited_crystal = LimitedCrystalCost,
                    limited_diamond = LimitedDiamondCost,
                    summon_point_id = banner.Id,
                    add_summon_point = AddSummonPoint,
                    add_summon_point_stone = AddSummonPointStone,
                    exchange_summon_point = ExchangeSummonPoint,
                    status = 1,
                    commence_date = banner.Start,
                    complete_date = banner.End,
                    daily_count = dailyCount,
                    daily_limit = DailyLimit,
                    total_count = totalCount,
                    total_limit = 0,
                }
            );
        }

        return results;
    }

    public async Task<IEnumerable<SummonTicketList>> GetSummonTicketList()
    {
        return await apiContext
            .PlayerSummonTickets.Where(x => x.ViewerId == playerIdentityService.ViewerId)
            .ProjectToSummonTicketList()
            .ToListAsync();
    }
}
