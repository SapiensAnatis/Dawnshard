using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using AutoMapper;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("summon")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class SummonController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;
    private readonly ISummonRepository summonRepository;
    private readonly ISessionService _sessionService;
    private readonly ISummonService _summonService;

    // Repeated from RedoableSummonController, but no point putting this in a shared location
    // as it's all bullshit anyway
    private static class Data
    {
        public static readonly OddsRate OddsRate =
            new(
                new List<AtgenRarityList>()
                {
                    new(5, "placeholder"),
                    new(4, "placeholder"),
                    new(3, "placeholder")
                },
                new List<AtgenRarityGroupList>()
                {
                    new(false, 5, "placeholder", "placeholder", "placeholder", "placeholder")
                },
                new(
                    new List<OddsUnitDetail>()
                    {
                        new(
                            false,
                            5,
                            new List<AtgenUnitList>() { new((int)Charas.Addis, "placeholder") }
                        )
                    },
                    new List<OddsUnitDetail>()
                    {
                        new(
                            false,
                            5,
                            new List<AtgenUnitList>() { new((int)Dragons.Agni, "placeholder") }
                        )
                    },
                    new List<OddsUnitDetail>()
                    {
                        new(
                            false,
                            5,
                            new List<AtgenUnitList>()
                            {
                                new(40050001, "lol you can still summon prints")
                            }
                        )
                    }
                )
            );

        public static readonly SummonPrizeOddsRate PrizeOddsRate =
            new(new List<AtgenSummonPrizeRankList>(), new List<AtgenSummonPrizeEntitySetList>());

        public static readonly SummonGetSummonListData SummonListData =
            new(
                new List<SummonList>()
                {
                    new(
                        summon_id: 1020203,
                        summon_type: 0,
                        summon_group_id: (int)BannerTypes.Normal,
                        single_crystal: 120,
                        single_diamond: 120,
                        multi_crystal: 1200,
                        multi_diamond: 1200,
                        limited_crystal: 5,
                        limited_diamond: 30,
                        summon_point_id: 1,
                        add_summon_point: 2,
                        exchange_summon_point: 300,
                        add_summon_point_stone: 1,
                        complete_date: (int)DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds(),
                        commence_date: (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        daily_count: 0,
                        daily_limit: 1,
                        total_count: 0,
                        total_limit: 0,
                        campaign_type: (int)SummonCampaignTypes.Normal,
                        free_count_rest: 0,
                        is_beginner_campaign: 1,
                        beginner_campaign_count_rest: 1,
                        consecution_campaign_count_rest: 0,
                        status: 0
                    )
                },
                new List<SummonList>(),
                new List<SummonList>(),
                new List<SummonList>(),
                new List<SummonList>(),
                new List<SummonList>(),
                new List<SummonList>(),
                new List<SummonList>(),
                new List<SummonList>(),
                new(
                    new List<SummonList>(),
                    new List<SummonList>(),
                    new List<SummonList>(),
                    new List<SummonList>(),
                    new List<SummonList>()
                ),
                new List<SummonTicketList>(),
                new List<SummonPointList>()
            );
    }

    public SummonController(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IUpdateDataService updateDataService,
        IMapper mapper,
        ISummonRepository summonRepository,
        ISessionService sessionService,
        ISummonService summonService
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
        this.summonRepository = summonRepository;
        _sessionService = sessionService;
        _summonService = summonService;
    }

    /// <summary>
    /// Returns excluded/excludable units for special banners
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("~/summon_exclude/get_list")]
    public async Task<DragaliaResult> SummonExcludeGetList(SummonExcludeGetListRequest request)
    {
        int bannerId = request.summon_id;
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();
        //TODO Replace DummyData with real exludes from BannerInfo
        List<AtgenDuplicateEntityList> excludableList = new();
        foreach (Charas c in Enum.GetValues<Charas>())
        {
            excludableList.Add(new AtgenDuplicateEntityList(EntityTypes.Chara, (int)c));
        }
        foreach (Dragons d in Enum.GetValues<Dragons>())
        {
            excludableList.Add(new AtgenDuplicateEntityList(EntityTypes.Dragon, (int)d));
        }

        return this.Ok(new SummonExcludeGetListData(excludableList));
    }

    [HttpPost]
    [Route("get_odds_data")]
    public async Task<DragaliaResult> GetOddsData(SummonGetOddsDataRequest request)
    {
        int bannerId = request.summon_id;
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();
        //TODO Replace Dummy data with oddscalculation

        return this.Ok(
            new SummonGetOddsDataData(
                new OddsRateList(0, Data.OddsRate, Data.OddsRate),
                new(Data.PrizeOddsRate, Data.PrizeOddsRate)
            )
        );
    }

    [HttpPost]
    [Route("get_summon_history")]
    public async Task<DragaliaResult> GetSummonHistory()
    {
        DbPlayerUserData userData = await userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();

        IEnumerable<SummonHistoryList> dbList = (
            await summonRepository.GetSummonHistory(this.DeviceAccountId)
        ).Select(mapper.Map<SummonHistoryList>);

        return Ok(new SummonGetSummonHistoryData(dbList));
    }

    [HttpPost]
    [Route("get_summon_list")]
    public async Task<DragaliaResult> GetSummonList([FromHeader(Name = "SID")] string sessionId)
    {
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

        // PS nano I removed all the DB stuff because I was too lazy to make it work again

        return Ok(Data.SummonListData);
    }

    [HttpPost]
    [Route("get_summon_point_trade")]
    public async Task<DragaliaResult> GetSummonPointTrade(
        [FromHeader(Name = "SID")] string sessionId,
        SummonGetSummonPointTradeRequest request
    )
    {
        int bannerId = request.summon_id;
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(accountId)
            .FirstAsync();
        //TODO maybe throw BadRequest on bad banner id, for now generate empty data if not exists
        DbPlayerBannerData playerBannerData = await this.summonRepository.GetPlayerBannerData(
            accountId,
            bannerId
        );
        //TODO get real list from persisted BannerInfo or dynamic List from Db, dunno yet
        List<AtgenSummonPointTradeList> tradableUnits =
            new()
            {
                new(bannerId * 1000 + 1, EntityTypes.Chara, (int)Charas.Celliera),
                new(bannerId * 1000 + 2, EntityTypes.Chara, (int)Charas.SummerCelliera)
            };

        return Ok(
            new SummonGetSummonPointTradeData(
                tradableUnits,
                new List<SummonPointList>() { new(bannerId, 0, 0, 0, int.MaxValue) },
                new(),
                new()
            )
        );
    }

    //TODO: Fully implement and refactor
    [HttpPost]
    [Route("request")]
    public async Task<DragaliaResult> RequestSummon(SummonRequestRequest summonRequest)
    {
        //TODO Fetch real data by bannerId
        SummonList bannerData =
            new(
                1020203,
                0,
                (int)BannerTypes.Normal,
                120,
                120,
                1200,
                1200,
                5,
                30,
                1,
                2,
                300,
                1,
                (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                (int)DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds(),
                0,
                1,
                0,
                0,
                (int)SummonCampaignTypes.Normal,
                0,
                1,
                1,
                0,
                0
            );

        DateTimeOffset summonDate = DateTimeOffset.UtcNow;

        DbPlayerBannerData playerBannerData = await this.summonRepository.GetPlayerBannerData(
            this.DeviceAccountId,
            bannerData.summon_id
        );

        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();

        int numSummons =
            summonRequest.exec_type == SummonExecTypes.Tenfold
                ? 10
                : Math.Max(1, summonRequest.exec_count);
        int summonPointMultiplier = bannerData.add_summon_point;
        int paymentHeld = 0;
        Action<int> paymentHeldSetter = reduction => { };
        int paymentCost = 1;
        switch (summonRequest.payment_type)
        {
            case PaymentTypes.Diamantium:
                summonPointMultiplier = bannerData.add_summon_point_stone;
                paymentHeld = 0;
                paymentHeldSetter = reduction =>
                {
                    playerBannerData.DailyLimitedSummonCount++;
                };
                paymentCost =
                    summonRequest.exec_type == SummonExecTypes.Tenfold
                        ? bannerData.multi_diamond
                        : bannerData.single_diamond * numSummons;
                break;
            case PaymentTypes.Wyrmite:
                paymentHeld = userData.Crystal;
                paymentHeldSetter = reduction => userData.Crystal -= reduction;
                paymentCost =
                    summonRequest.exec_type == SummonExecTypes.Tenfold
                        ? bannerData.multi_crystal
                        : bannerData.single_crystal * numSummons;
                break;
            case PaymentTypes.Ticket:
                paymentCost =
                    summonRequest.exec_type == SummonExecTypes.Tenfold
                        ? 1
                        : summonRequest.exec_count * numSummons;
                break;
            case PaymentTypes.FreeDailyExecDependant:
            case PaymentTypes.FreeDailyTenfold:
                paymentHeldSetter = x =>
                {
                    if (bannerData.is_beginner_campaign == 1)
                        playerBannerData.IsBeginnerFreeSummonAvailable = 0;
                };
                paymentCost = 0;
                break;
            default:
                return BadRequest();
        }

        if (paymentHeld < paymentCost)
            return BadRequest();

        List<AtgenRedoableSummonResultUnitList> summonResult = _summonService.GenerateSummonResult(
            numSummons
        );
        //TODO: Roll prize summon and commit prize summon results

        await this.unitRepository.AddCharas(
            this.DeviceAccountId,
            summonResult
                .Where(x => x.entity_type == EntityTypes.Chara)
                .Select(x => (Charas)x.id)
        );

        await this.unitRepository.AddDragons(
            this.DeviceAccountId,
            summonResult
                .Where(x => x.entity_type == EntityTypes.Dragon)
                .Select(x => (Dragons)x.id)
        );

        IEnumerable<AtgenResultUnitList> rewardList = _summonService.GenerateRewardList(
            this.DeviceAccountId,
            summonResult
        );

        paymentHeldSetter(paymentCost);
        playerBannerData.SummonPoints += numSummons * summonPointMultiplier;
        playerBannerData.SummonCount += numSummons;

        List<AtgenDuplicateEntityList> newEntities = rewardList
            .Where(x => x.is_new)
            .Select(x => new AtgenDuplicateEntityList(x.entity_type, x.id))
            .ToList();

        List<DbPlayerSummonHistory> historyEntries = new();

        int lastIndexOfRare5 = -1;
        int countOfRare5Char = 0;
        int countOfRare5Dragon = 0;
        int countOfRare4 = 0;
        int topRarity = 3;

        for (int i = 0; i < summonResult.Count; i++)
        {
            AtgenRedoableSummonResultUnitList summon = summonResult[i];
            topRarity = topRarity > summon.rarity ? topRarity : summon.rarity;
            EntityTypes entityType = (EntityTypes)summon.entity_type;
            if (summon.rarity == 5)
            {
                lastIndexOfRare5 = i;
                if (entityType == EntityTypes.Chara)
                    countOfRare5Char++;
                else if (entityType == EntityTypes.Dragon)
                {
                    countOfRare5Dragon++;
                }
            }
            if (summon.rarity == 4)
                countOfRare4++;

            bool isNew =
                newEntities.Find(
                    x => x.entity_type == summon.entity_type && x.entity_id == summon.id
                ) != null
                && !rewardList.Any(x => x.entity_type == summon.entity_type && x.id == summon.id);

            int dewGained = 0;
            if (!isNew && summon.entity_type == EntityTypes.Chara)
            {
                dewGained = DewValueData.DupeSummon[summon.rarity];
                userData.DewPoint += dewGained;
            }

            //TODO: Get prize rank for this reward
            SummonPrizeRanks prizeRank = SummonPrizeRanks.None;

            historyEntries.Add(
                new DbPlayerSummonHistory()
                {
                    DeviceAccountId = this.DeviceAccountId,
                    SummonId = bannerData.summon_id,
                    SummonExecType = summonRequest.exec_type,
                    ExecDate = summonDate,
                    PaymentType = summonRequest.payment_type,
                    EntityType = (EntityTypes)summon.entity_type,
                    EntityId = summon.id,
                    EntityQuantity = 1,
                    EntityLevel = 1,
                    EntityRarity = (byte)summon.rarity,
                    EntityLimitBreakCount = 0,
                    EntityHpPlusCount = 0,
                    EntityAttackPlusCount = 0,
                    SummonPrizeRank = prizeRank,
                    SummonPoint = summonPointMultiplier,
                    GetDewPointQuantity = dewGained
                }
            );
        }

        await summonRepository.AddSummonHistory(historyEntries);

        int reversalIndex = lastIndexOfRare5;
        if (reversalIndex != -1 && new Random().NextSingle() < 0.95)
            reversalIndex = -1;

        int sageEffect = (int)SummonEffectsSage.Dull;
        int circleEffect = (int)SummonEffectsSky.Blue;
        int rarityDisplayModifier = reversalIndex == -1 ? 0 : 1;
        if (countOfRare5Char + countOfRare5Dragon > 0 + rarityDisplayModifier)
        {
            sageEffect =
                countOfRare5Dragon > countOfRare5Char
                    ? (int)SummonEffectsSage.GoldFafnirs
                    : (int)SummonEffectsSage.RainbowCrystal;
            circleEffect = (int)SummonEffectsSky.Rainbow;
        }
        else
        {
            circleEffect = (int)SummonEffectsSky.Yellow;
            switch (countOfRare4 + (countOfRare5Char + countOfRare5Dragon) * 2)
            {
                case > 1:
                    sageEffect = (int)SummonEffectsSage.MultiDoves;
                    break;
                case > 0:
                    sageEffect = (int)SummonEffectsSage.SingleDove;
                    break;
                default:
                    sageEffect = (int)SummonEffectsSage.Dull;
                    circleEffect = (int)SummonEffectsSky.Blue;
                    break;
            }
        }

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(this.DeviceAccountId);
        await this.summonRepository.SaveChangesAsync();

        return this.Ok(
            new SummonRequestData(
                rewardList,
                new List<AtgenResultPrizeList>(),
                new List<int>() { sageEffect, circleEffect },
                reversalIndex,
                updateDataList,
                new EntityResult(),
                new List<SummonTicketList>(),
                playerBannerData.SummonPoints,
                new List<UserSummonList>()
                {
                    new(
                        bannerData.summon_id,
                        playerBannerData.SummonCount,
                        bannerData.campaign_type,
                        bannerData.free_count_rest,
                        bannerData.is_beginner_campaign,
                        bannerData.beginner_campaign_count_rest,
                        bannerData.consecution_campaign_count_rest
                    )
                }
            )
        );
    }
}
