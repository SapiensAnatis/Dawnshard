using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.Summon;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.Features.Summoning;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using FluentRandomPicker;
using FluentRandomPicker.FluentInterfaces.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Summoning;

public sealed partial class SummonService(
    SummonOddsService summonOddsService,
    UnitService unitService,
    IOptionsMonitor<SummonBannerOptions> optionsMonitor,
    IPlayerIdentityService playerIdentityService,
    IPresentService presentService,
    IRewardService rewardService,
    IPaymentService paymentService,
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

    public async Task<IList<SummonTicketList>> GetSummonTicketList() =>
        await apiContext.PlayerSummonTickets.ProjectToSummonTicketList().ToListAsync();

    public async Task<IList<SummonList>> GetSummonList()
    {
        List<SummonList> results = new(optionsMonitor.CurrentValue.Banners.Count);

        var individualDataDict = await apiContext
            .PlayerBannerData.Select(x => new
            {
                BannerId = x.SummonBannerId,
                DailyCount = x.DailyLimitedSummonCount,
                TotalCount = x.SummonCount,
            })
            .ToDictionaryAsync(x => x.BannerId, x => x);

        Dictionary<SummonTickets, int> summonTicketDataDict = await apiContext
            .PlayerSummonTickets.GroupBy(x => x.SummonTicketId)
            .ToDictionaryAsync(x => x.Key, x => x.Sum(y => y.Quantity));

        foreach (Banner banner in optionsMonitor.CurrentValue.Banners)
        {
            if (banner.Id == SummonConstants.RedoableSummonBannerId)
            {
                continue;
            }

            if (banner.RequiredTicketId != null)
            {
                int ownedQuantity = summonTicketDataDict.GetValueOrDefault(
                    banner.RequiredTicketId.Value,
                    0
                );

                if (ownedQuantity <= 0)
                {
                    continue;
                }
            }

            int dailyCount = 0;
            int totalCount = 0;

            if (individualDataDict.TryGetValue(banner.Id, out var bannerData))
            {
                dailyCount = bannerData.DailyCount;
                totalCount = bannerData.TotalCount;
            }

            SummonList item =
                new()
                {
                    SummonId = banner.Id,
                    SummonType = banner.SummonType,
                    Status = 1,
                    CommenceDate = banner.Start,
                    CompleteDate = banner.End,
                    DailyCount = dailyCount,
                    TotalCount = totalCount,
                };

            if (banner.SummonType == SummonTypes.Normal)
            {
                item.SummonPointId = banner.Id;
                item.SingleCrystal = SummonConstants.SingleCrystalCost;
                item.SingleDiamond = SummonConstants.SingleDiamondCost;
                item.MultiCrystal = SummonConstants.MultiCrystalCost;
                item.MultiDiamond = SummonConstants.MultiDiamondCost;
                item.LimitedCrystal = SummonConstants.LimitedCrystalCost;
                item.LimitedDiamond = SummonConstants.LimitedDiamondCost;
                item.AddSummonPoint = SummonConstants.AddSummonPoint;
                item.AddSummonPointStone = SummonConstants.AddSummonPointStone;
                item.ExchangeSummonPoint = SummonConstants.ExchangeSummonPoint;
                item.DailyLimit = 1;
            }

            results.Add(item);
        }

        return results;
    }

    public async Task<SummonList?> GetSummonList(int bannerId)
    {
        // Note: could be written to only fetch player data for a single banner to be more efficient. Should still be
        // one query so not a high priority for now.
        if (bannerId == SummonConstants.RedoableSummonBannerId)
            return null;

        IList<SummonList> availableBanners = await GetSummonList();
        return availableBanners.FirstOrDefault(x => x.SummonId == bannerId);
    }

    public async Task<IList<SummonPointList>> GetSummonPointList()
    {
        // We should not return summon point information for special banners,
        // like the 5* voucher ones, because this causes a 0 > 0 wyrmsigil reward
        // pop-up after summoning.
        List<int> searchBannerIds = optionsMonitor
            .CurrentValue.Banners.Where(x => x.SummonType == SummonTypes.Normal)
            .Select(x => x.Id)
            .ToList();

        return await apiContext
            .PlayerBannerData.Where(x => searchBannerIds.Contains(x.SummonBannerId))
            .Select(x => new SummonPointList()
            {
                SummonPointId = x.SummonBannerId,
                SummonPoint = x.SummonPoints,
                CsSummonPoint = x.ConsecutionSummonPoints,
                CsPointTermMinDate = x.ConsecutionSummonPointsMinDate,
                CsPointTermMaxDate = x.ConsecutionSummonPointsMaxDate
            })
            .ToListAsync();
    }

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

    public async Task<IList<SummonHistoryList>> GetSummonHistory() =>
        await apiContext
            .PlayerSummonHistory.Take(30) // See: https://dragalialost.wiki/w/Version_Changelog/Ver_1.18.0_Version_Update#Summon_History
            .Select(x => x.ToSummonHistoryList())
            .ToListAsync();

    public void AddSummonHistory(
        SummonList summonList,
        SummonRequestRequest summonRequest,
        AtgenResultUnitList result
    ) =>
        apiContext.PlayerSummonHistory.Add(
            new DbPlayerSummonHistory()
            {
                ViewerId = playerIdentityService.ViewerId,
                SummonId = summonList.SummonId,
                SummonExecType = summonRequest.ExecType,
                ExecDate = DateTimeOffset.UtcNow,
                PaymentType = summonRequest.PaymentType,
                EntityType = result.EntityType,
                EntityId = result.Id,
                EntityQuantity = 1,
                EntityLevel = 1,
                EntityRarity = (byte)result.Rarity,
                EntityLimitBreakCount = 0,
                EntityHpPlusCount = 0,
                EntityAttackPlusCount = 0,
                SummonPrizeRank = SummonPrizeRanks.None,
                SummonPoint =
                    summonRequest.PaymentType == PaymentTypes.Diamantium
                        ? summonList.AddSummonPointStone
                        : summonList.AddSummonPoint,
                GetDewPointQuantity = result.DewPoint,
            }
        );

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

        if (result == RewardGrantResult.Limit)
        {
            Log.AddingTradeToGiftBox(logger, tradeList);
            presentService.AddPresent(
                new Present.Present(
                    PresentMessage.WyrmsigilReward,
                    tradeList.EntityType,
                    tradeList.EntityId
                )
            );
        }
        else if (result != RewardGrantResult.Added)
        {
            Log.UnexpectedRewardResult(logger, result);
        }

        return entity.ToBuildEventRewardEntityList();
    }

    public async Task ProcessSummonPayment(
        SummonRequestRequest summonRequest,
        SummonList summonList
    )
    {
        int execCount = summonRequest.ExecCount > 0 ? summonRequest.ExecCount : 1;

        int paymentCost = (summonRequest.PaymentType, summonRequest.ExecType) switch
        {
            (PaymentTypes.Diamantium, SummonExecTypes.Tenfold) => summonList.MultiDiamond,
            (PaymentTypes.Diamantium, SummonExecTypes.Single)
                => summonList.SingleDiamond * execCount,
            (PaymentTypes.Wyrmite, SummonExecTypes.Tenfold) => summonList.MultiCrystal,
            (PaymentTypes.Wyrmite, SummonExecTypes.Single) => summonList.SingleCrystal * execCount,
            (PaymentTypes.Ticket, SummonExecTypes.Tenfold) => 1,
            (PaymentTypes.Ticket, SummonExecTypes.Single) => execCount,
            (PaymentTypes.FreeDailyTenfold, SummonExecTypes.Tenfold) => 0,
            _
                => throw new DragaliaException(
                    ResultCode.SummonTypeUnexpected,
                    $"Failed to calculate summon cost for payment type {summonRequest.PaymentType} and exec type {summonRequest.ExecType}"
                )
        };

        int entityId = 0;

        if (summonRequest.PaymentType == PaymentTypes.Ticket)
        {
            if (
                optionsMonitor.CurrentValue.Banners.First(x => x.Id == summonRequest.SummonId) is
                { RequiredTicketId: { } requiredTicket }
            )
            {
                entityId = (int)requiredTicket;
            }
            else
            {
                SummonTickets genericTicketId = summonRequest.ExecType switch
                {
                    SummonExecTypes.Single => SummonTickets.SingleSummon,
                    SummonExecTypes.Tenfold => SummonTickets.TenfoldSummon,
                    _
                        => throw new DragaliaException(
                            ResultCode.CommonInvalidArgument,
                            $"Invalid exec type {summonRequest.ExecType} for ticket summon"
                        )
                };

                entityId = (int)genericTicketId;
            }
        }

        await paymentService.ProcessPayment(
            new Entity(summonRequest.PaymentType.ToEntityType(), entityId, paymentCost),
            summonRequest.PaymentTarget
        );
    }

    public async Task<(
        IList<AtgenResultUnitList> ResultUnitList,
        SummonResultMetaInfo MetaInfo,
        EntityResult EntityResult
    )> CommitSummonResult(IList<AtgenRedoableSummonResultUnitList> summonResult)
    {
        List<AtgenResultUnitList> returnedResult = [];
        List<AtgenDuplicateEntityList> newGetEntityList = [];

        int lastIndexOfRare5 = 0;
        int countOfRare5Char = 0;
        int countOfRare5Dragon = 0;
        int countOfRare4 = 0;

        List<Dragons> dragonList = summonResult
            .Where(x => x.EntityType == EntityTypes.Dragon)
            .Select(x => (Dragons)x.Id)
            .ToList();

        List<Charas> charaList = summonResult
            .Where(x => x.EntityType == EntityTypes.Chara)
            .Select(x => (Charas)x.Id)
            .ToList();

        List<Dragons> newDragons = (await unitService.AddDragons(dragonList))
            .Where(x => x.IsNew)
            .Select(x => x.Id)
            .ToList();

        List<Charas> newCharas = (await unitService.AddCharas(charaList))
            .Where(x => x.IsNew)
            .Select(x => x.Id)
            .ToList();

        DbPlayerUserData userData = await apiContext.PlayerUserData.FirstAsync();

        foreach (
            (AtgenRedoableSummonResultUnitList result, int index) in summonResult.Select(
                (x, i) => (x, i)
            )
        )
        {
            bool isNew = result.EntityType switch
            {
                EntityTypes.Dragon => newDragons.Remove((Dragons)result.Id),
                EntityTypes.Chara => newCharas.Remove((Charas)result.Id),
                _ => throw new UnreachableException("Invalid entity type"),
            };

            int dewPoint = 0;
            if (!isNew && result.EntityType is EntityTypes.Chara)
            {
                dewPoint = CalculateDewValue((Charas)result.Id);
                userData.DewPoint += dewPoint;
            }
            else
            {
                newGetEntityList.Add(
                    new AtgenDuplicateEntityList
                    {
                        EntityType = result.EntityType,
                        EntityId = result.Id
                    }
                );
            }

            switch (result.Rarity)
            {
                case 5:
                {
                    lastIndexOfRare5 = index;
                    if (result.EntityType is EntityTypes.Chara)
                        countOfRare5Char++;
                    else
                        countOfRare5Dragon++;
                    break;
                }
                case 4:
                    countOfRare4++;
                    break;
            }

            AtgenResultUnitList processedResult =
                new()
                {
                    EntityType = result.EntityType,
                    Id = result.Id,
                    IsNew = isNew,
                    Rarity = result.Rarity,
                    DewPoint = dewPoint,
                };

            returnedResult.Add(processedResult);
        }

        IEnumerable<AtgenBuildEventRewardEntityList> overPresentEntityList = presentService
            .AddedPresents.Where(x => x.EntityType == EntityTypes.Dragon)
            .Select(x => x.ToBuildEventRewardList());

        return (
            returnedResult,
            new SummonResultMetaInfo(
                LastIndexOfRare5: lastIndexOfRare5,
                CountOfRare5Char: countOfRare5Char,
                CountOfRare5Dragon: countOfRare5Dragon,
                CountOfRare4: countOfRare4
            ),
            new EntityResult()
            {
                // We are not using the RewardService method as it would include OverDiscardEntityList for any characters that were already owned.
                NewGetEntityList = newGetEntityList,
                OverPresentEntityList = overPresentEntityList,
            }
        );

        static int CalculateDewValue(Charas id)
        {
            CharaData data = MasterAsset.CharaData[id];
            return data.GetAvailability() == UnitAvailability.Story
                ? DewValueData.DupeStorySummon[data.Rarity]
                : DewValueData.DupeSummon[data.Rarity];
        }
    }

    public async Task<UserSummonList> UpdateUserSummonInformation(
        SummonList summonList,
        int summonCount
    )
    {
        DbPlayerBannerData? playerBannerData = await this.GetPlayerBannerData(summonList.SummonId);

        if (playerBannerData is null)
        {
            throw new InvalidOperationException(
                $"PlayerBannerData for banner {summonList.SummonId} was not found"
            );
        }

        int gainedSummonPoints = summonCount * summonList.AddSummonPoint;
        playerBannerData.SummonPoints += gainedSummonPoints;
        playerBannerData.SummonCount += summonCount;

        return new UserSummonList()
        {
            SummonId = summonList.SummonId,
            SummonCount = playerBannerData.SummonCount,
            CampaignType = summonList.CampaignType,
            FreeCountRest = summonList.FreeCountRest,
            IsBeginnerCampaign = summonList.IsBeginnerCampaign,
            BeginnerCampaignCountRest = summonList.BeginnerCampaignCountRest,
            ConsecutionCampaignCountRest = summonList.ConsecutionCampaignCountRest
        };
    }

    public readonly record struct SummonResultMetaInfo(
        int LastIndexOfRare5,
        int CountOfRare5Char,
        int CountOfRare5Dragon,
        int CountOfRare4
    );

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
