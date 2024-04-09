using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using FluentRandomPicker;
using FluentRandomPicker.FluentInterfaces.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Summoning;

public sealed partial class SummonService(
    SummonOddsService summonOddsService,
    IOptionsMonitor<SummonBannerOptions> optionsMonitor,
    IUnitRepository unitRepository,
    IPlayerIdentityService playerIdentityService,
    IRewardService rewardService,
    ApiContext apiContext,
    ILogger<SummonService> logger
)
{
    public Task<List<AtgenRedoableSummonResultUnitList>> GenerateSummonResult(
        int numSummons,
        int bannerId,
        SummonExecTypes execType
    ) =>
        execType switch
        {
            SummonExecTypes.Single
            or SummonExecTypes.DailyDeal
                => this.GenerateSummonResultInternal(bannerId, numSummons),
            SummonExecTypes.Tenfold => this.GenerateTenfoldResultInternal(bannerId, numTenfolds: 1),
            _ => throw new ArgumentException($"Invalid summon type {execType}", nameof(execType)),
        };

    public Task<List<AtgenRedoableSummonResultUnitList>> GenerateRedoableSummonResult() =>
        this.GenerateTenfoldResultInternal(SummonConstants.RedoableSummonBannerId, numTenfolds: 5);

    /// <summary>
    /// Populate a summon result with is_new and eldwater values.
    /// </summary>
    public async Task<List<AtgenResultUnitList>> GenerateRewardList(
        IEnumerable<AtgenRedoableSummonResultUnitList> baseRewardList
    )
    {
        List<AtgenResultUnitList> newUnits = new();

        List<Charas> ownedCharas = await apiContext
            .PlayerCharaData.Select(x => x.CharaId)
            .ToListAsync();

        List<Dragons> ownedDragons = await unitRepository
            .Dragons.Select(x => x.DragonId)
            .ToListAsync();

        foreach (AtgenRedoableSummonResultUnitList reward in baseRewardList)
        {
            bool isNew = newUnits.All(x => x.Id != reward.Id);

            switch (reward.EntityType)
            {
                case EntityTypes.Chara:
                {
                    isNew |= ownedCharas.All(x => x != (Charas)reward.Id);

                    AtgenResultUnitList toAdd =
                        new(
                            reward.EntityType,
                            reward.Id,
                            reward.Rarity,
                            isNew,
                            3,
                            isNew ? 0 : DewValueData.DupeSummon[reward.Rarity]
                        );

                    newUnits.Add(toAdd);
                    break;
                }
                case EntityTypes.Dragon:
                {
                    isNew |= ownedDragons.All(x => x != (Dragons)reward.Id);

                    AtgenResultUnitList toAdd =
                        new(reward.EntityType, reward.Id, reward.Rarity, isNew, 3, 0);

                    newUnits.Add(toAdd);
                    break;
                }
                default:
                    throw new UnreachableException(
                        "Invalid entity type for redoable summon result."
                    );
            }
        }

        return newUnits;
    }

    public async Task<IEnumerable<SummonTicketList>> GetSummonTicketList() =>
        await apiContext.PlayerSummonTickets.ProjectToSummonTicketList().ToListAsync();

    public async Task<List<SummonList>> GetSummonList()
    {
        List<SummonList> results = new(optionsMonitor.CurrentValue.Banners.Count);

        var individualDataDict = await apiContext
            .PlayerBannerData.Where(x => x.ViewerId == playerIdentityService.ViewerId)
            .Select(x => new
            {
                BannerId = x.SummonBannerId,
                DailyCount = x.DailyLimitedSummonCount,
                TotalCount = x.SummonCount,
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
                    SummonPointId = banner.Id,
                    SummonType = 2, // No idea what this does.
                    SingleCrystal = SummonConstants.SingleCrystalCost,
                    SingleDiamond = SummonConstants.SingleDiamondCost,
                    MultiCrystal = SummonConstants.MultiCrystalCost,
                    MultiDiamond = SummonConstants.MultiDiamondCost,
                    LimitedCrystal = SummonConstants.LimitedCrystalCost,
                    LimitedDiamond = SummonConstants.LimitedDiamondCost,
                    AddSummonPoint = SummonConstants.AddSummonPoint,
                    AddSummonPointStone = SummonConstants.AddSummonPointStone,
                    ExchangeSummonPoint = SummonConstants.ExchangeSummonPoint,
                    DailyLimit = SummonConstants.DailyLimit,
                    Status = 1,
                    CommenceDate = banner.Start,
                    CompleteDate = banner.End,
                    DailyCount = dailyCount,
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

    public async Task<List<SummonPointList>> GetSummonPointList() =>
        await apiContext
            .PlayerBannerData.Select(x => new SummonPointList()
            {
                SummonPointId = x.SummonBannerId,
                SummonPoint = x.SummonPoints,
                CsSummonPoint = x.ConsecutionSummonPoints,
                CsPointTermMinDate = x.ConsecutionSummonPointsMinDate,
                CsPointTermMaxDate = x.ConsecutionSummonPointsMaxDate
            })
            .ToListAsync();

    public async Task<SummonPointList?> GetSummonPointList(int summonBannerId)
    {
        DbPlayerBannerData? bannerData = await this.GetPlayerBannerData(summonBannerId);

        if (bannerData is null)
        {
            return null;
        }

        return new SummonPointList()
        {
            SummonPointId = bannerData.SummonBannerId,
            SummonPoint = bannerData.SummonPoints,
            CsSummonPoint = bannerData.ConsecutionSummonPoints,
            CsPointTermMinDate = bannerData.ConsecutionSummonPointsMinDate,
            CsPointTermMaxDate = bannerData.ConsecutionSummonPointsMaxDate
        };
    }

    public async Task<DbPlayerBannerData?> GetPlayerBannerData(int bannerId)
    {
        if (
            optionsMonitor.CurrentValue.Banners.FirstOrDefault(x => x.Id == bannerId)
            is not { } banner
        )
        {
            return null;
        }

        DbPlayerBannerData? bannerData = await apiContext.PlayerBannerData.FirstOrDefaultAsync(x =>
            x.SummonBannerId == bannerId
        );

        if (bannerData is null)
        {
            bannerData = new()
            {
                ViewerId = playerIdentityService.ViewerId,
                SummonBannerId = banner.Id,
                ConsecutionSummonPointsMinDate = DateTimeOffset.UtcNow,
                ConsecutionSummonPointsMaxDate = banner.End,
            };

            apiContext.PlayerBannerData.Add(bannerData);
        }

        return bannerData;
    }

    public IEnumerable<AtgenSummonPointTradeList>? GetSummonPointTradeList(int bannerId)
    {
        if (
            optionsMonitor.CurrentValue.Banners.FirstOrDefault(x => x.Id == bannerId)
            is not { } banner
        )
        {
            return null;
        }

        return banner.TradeDictionary.Values;
    }

    public async Task<AtgenBuildEventRewardEntityList> DoSummonPointTrade(int summonId, int tradeId)
    {
        if (
            optionsMonitor.CurrentValue.Banners.FirstOrDefault(x => x.Id == summonId)
            is not { } banner
        )
        {
            throw new DragaliaException(
                ResultCode.SummonNotFound,
                $"Failed to find summon {summonId}"
            );
        }

        if (!banner.TradeDictionary.TryGetValue(tradeId, out AtgenSummonPointTradeList? tradeList))
        {
            throw new DragaliaException(
                ResultCode.SummonNotFound,
                $"Failed to find summon trade {tradeId} for summon {summonId}"
            );
        }

        DbPlayerBannerData bannerData = await apiContext.PlayerBannerData.FirstAsync(x =>
            x.SummonBannerId == banner.Id
        );

        // TODO: Some banners had a lower spark cost. We should eventually retrieve the cost from the banner itself.

        if (bannerData.SummonPoints < SummonConstants.ExchangeSummonPoint)
        {
            throw new DragaliaException(
                ResultCode.SummonPointTradePointShort,
                $"Failed to perform summon trade: point quantity {bannerData.SummonPoints} is insufficient"
            );
        }

        Log.PerformingPointTrade(logger, tradeList);

        bannerData.SummonPoints -= SummonConstants.ExchangeSummonPoint;
        Entity entity = new(tradeList.EntityType, tradeList.EntityId);
        RewardGrantResult result = await rewardService.GrantReward(entity);

        Log.PointsDeducted(logger, bannerData.SummonPoints);

        if (
            result
            is not (
                RewardGrantResult.Added
                or RewardGrantResult.GiftBox
                or RewardGrantResult.GiftBoxDiscarded
            )
        )
        {
            Log.UnexpectedRewardResult(logger, result);
        }

        return entity.ToBuildEventRewardEntityList();
    }

    private async Task<List<AtgenRedoableSummonResultUnitList>> GenerateSummonResultInternal(
        int bannerId,
        int numSummons
    )
    {
        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rateData =
            await summonOddsService.GetUnitRates(bannerId);

        List<UnitRate> allRates = [.. rateData.PickupRates, .. rateData.NormalRates];

        IPick<AtgenRedoableSummonResultUnitList> picker = Out.Of()
            .PrioritizedElements(allRates)
            .WithValueSelector(ToSummonResult)
            .AndWeightSelector(x => x.Weighting);

        List<AtgenRedoableSummonResultUnitList> result = new(numSummons);

        for (int i = 0; i < numSummons; i++)
            result.Add(picker.PickOne());

        return result;
    }

    private async Task<List<AtgenRedoableSummonResultUnitList>> GenerateTenfoldResultInternal(
        int bannerId,
        int numTenfolds
    )
    {
        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) normalRateData =
            await summonOddsService.GetUnitRates(bannerId);
        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) guaranteeRateData =
            await summonOddsService.GetGuaranteeUnitRates(bannerId);

        List<UnitRate> allNormalRates =
        [
            .. normalRateData.PickupRates,
            .. normalRateData.NormalRates
        ];
        List<UnitRate> allGuaranteeRates =
        [
            .. guaranteeRateData.PickupRates,
            .. guaranteeRateData.NormalRates
        ];

        IPick<AtgenRedoableSummonResultUnitList> normalPicker = Out.Of()
            .PrioritizedElements(allNormalRates)
            .WithValueSelector(ToSummonResult)
            .AndWeightSelector(x => x.Weighting);

        IPick<AtgenRedoableSummonResultUnitList> guaranteePicker = Out.Of()
            .PrioritizedElements(allGuaranteeRates)
            .WithValueSelector(ToSummonResult)
            .AndWeightSelector(x => x.Weighting);

        List<AtgenRedoableSummonResultUnitList> result = new(10);

        for (int tenfold = 0; tenfold < numTenfolds; tenfold++)
        {
            for (int i = 0; i < 9; i++)
                result.Add(normalPicker.PickOne());

            result.Add(guaranteePicker.PickOne());
        }

        return result;
    }

    private static AtgenRedoableSummonResultUnitList ToSummonResult(UnitRate rate) =>
        new()
        {
            Id = rate.Id,
            EntityType = rate.EntityType,
            Rarity = rate.Rarity,
        };
}
