using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using AutoMapper;

namespace DragaliaAPI.Controllers.Dragalia.Summon;

[Route("summon")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class SummonController : ControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IMapper mapper;
    private readonly ISummonRepository summonRepository;
    private readonly ISessionService _sessionService;
    private readonly ISummonService _summonService;

    public SummonController(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IMapper mapper,
        ISummonRepository summonRepository,
        ISessionService sessionService,
        ISummonService summonService
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.mapper = mapper;
        this.summonRepository = summonRepository;
        _sessionService = sessionService;
        _summonService = summonService;
    }

    /// <summary>
    /// Returns excluded/excludable units for special banners
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="bannerIdRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("~/summon_exclude/get_list")]
    public async Task<DragaliaResult> SummonExcludeGetList(
        [FromHeader(Name = "SID")] string sessionId,
        BannerIdRequest bannerIdRequest
    )
    {
        int bannerId = bannerIdRequest.summon_id;
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(accountId)
            .FirstAsync();
        //TODO Replace DummyData with real exludes from BannerInfo
        List<BaseNewEntity> excludableList = new();
        foreach (Charas c in Enum.GetValues<Charas>())
        {
            excludableList.Add(new BaseNewEntity((int)EntityTypes.Chara, (int)c));
        }
        foreach (Dragons d in Enum.GetValues<Dragons>())
        {
            excludableList.Add(new BaseNewEntity((int)EntityTypes.Dragon, (int)d));
        }
        return this.Ok(
            new SummonExcludeGetListResponse(
                SummonExcludeGetListResponseFactory.CreateData(
                    excludableList,
                    new UpdateDataList() { user_data = SavefileUserDataFactory.Create(userData) }
                )
            )
        );
    }

    [HttpPost]
    [Route("get_odds_data")]
    public async Task<DragaliaResult> GetOddsData(
        [FromHeader(Name = "SID")] string sessionId,
        BannerIdRequest bannerIdRequest
    )
    {
        int bannerId = bannerIdRequest.summon_id;
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(accountId)
            .FirstAsync();
        //TODO Replace Dummy data with oddscalculation
        return this.Ok(
            new SummonGetOddsDataResponse(
                SummonGetOddsDataResponseFactory.CreateDummyData(
                    SavefileUserDataFactory.Create(userData)
                )
            )
        );
    }

    [HttpPost]
    [Route("get_summon_history")]
    public async Task<DragaliaResult> GetSummonHistory([FromHeader(Name = "SID")] string sessionId)
    {
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbPlayerUserData userData = await userDataRepository.GetUserData(accountId).FirstAsync();
        List<SummonHistory> dbList = (await summonRepository.GetSummonHistory(accountId))
            .Select(mapper.Map<SummonHistory>)
            .ToList();
        return Ok(
            new SummonGetSummonHistoryResponse(
                SummonGetSummonHistoryResponseFactory.CreateData(
                    dbList,
                    new UpdateDataList() { user_data = SavefileUserDataFactory.Create(userData) }
                )
            )
        );
    }

    [HttpPost]
    [Route("get_summon_list")]
    public async Task<DragaliaResult> GetSummonList([FromHeader(Name = "SID")] string sessionId)
    {
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        SummonGetSummonListResponseData data = SummonGetSummonListResponseFactory.CreateDummyData();
        //TODO Replace DummyData
        DbPlayerBannerData dummy = await this.summonRepository.GetPlayerBannerData(
            accountId,
            1020203
        );
        data.summon_point_list.Add(
            new(
                dummy.SummonBannerId,
                dummy.SummonPoints,
                dummy.ConsecutionSummonPoints,
                dummy.ConsecutionSummonPointsMinDate,
                dummy.ConsecutionSummonPointsMaxDate
            )
        );
        dummy = await this.summonRepository.GetPlayerBannerData(accountId, 1110003);
        data.summon_point_list.Add(
            new(
                dummy.SummonBannerId,
                dummy.SummonPoints,
                dummy.ConsecutionSummonPoints,
                dummy.ConsecutionSummonPointsMinDate,
                dummy.ConsecutionSummonPointsMaxDate
            )
        );
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(accountId)
            .FirstAsync();
        data = data with
        {
            update_data_list = new UpdateDataList()
            {
                user_data = SavefileUserDataFactory.Create(userData)
            }
        };
        return Ok(new SummonGetSummonListResponse(data));
    }

    [HttpPost]
    [Route("get_summon_point_trade")]
    public async Task<DragaliaResult> GetSummonPointTrade(
        [FromHeader(Name = "SID")] string sessionId,
        BannerIdRequest bannerIdRequest
    )
    {
        int bannerId = bannerIdRequest.summon_id;
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
        List<TradableEntity> tradableUnits =
            new()
            {
                new TradableEntity(
                    (bannerId * 1000) + 1,
                    (int)EntityTypes.Chara,
                    (int)Charas.Celliera
                ),
                new TradableEntity(
                    (bannerId * 1000) + 2,
                    (int)EntityTypes.Chara,
                    (int)Charas.SummerCelliera
                )
            };

        SummonGetSummonPointTradeResponse response = new SummonGetSummonPointTradeResponse(
            SummonGetSummonPointTradeResponseResponseFactory.CreateData(
                tradableUnits,
                new(
                    bannerId,
                    playerBannerData.SummonPoints,
                    playerBannerData.ConsecutionSummonPoints,
                    playerBannerData.ConsecutionSummonPointsMinDate,
                    playerBannerData.ConsecutionSummonPointsMaxDate
                ),
                SavefileUserDataFactory.Create(userData)
            )
        );
        return Ok(response);
    }

    //TODO: Fully implement and refactor
    [HttpPost]
    [Route("request")]
    public async Task<DragaliaResult> RequestSummon(
        [FromHeader(Name = "SID")] string sessionId,
        SummonRequest summonRequest
    )
    {
        //TODO Fetch real data by bannerId
        BannerData bannerData = new BannerData(
            1020203,
            null,
            BannerTypes.Normal,
            120,
            120,
            1200,
            1200,
            30,
            30,
            1,
            2,
            300,
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(7),
            0,
            0,
            0,
            0,
            SummonCampaignTypes.Normal,
            0,
            false,
            false,
            0
        );

        DateTimeOffset summonDate = DateTimeOffset.UtcNow;

        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

        DbPlayerBannerData playerBannerData = await this.summonRepository.GetPlayerBannerData(
            accountId,
            bannerData.summon_id
        );

        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(accountId)
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
                    if (bannerData.is_beginner_campaign)
                    {
                        playerBannerData.IsBeginnerFreeSummonAvailable = 0;
                    }
                };
                paymentCost = 0;
                break;
            default:
                return BadRequest();
        }

        if (paymentHeld < paymentCost)
        {
            return BadRequest();
        }

        List<SimpleSummonReward> summonResult = _summonService.GenerateSummonResult(numSummons);
        //TODO: Roll prize summon and commit prize summon results

        IEnumerable<DbPlayerCharaData> repositoryCharaOuput = await this.unitRepository.AddCharas(
            accountId,
            summonResult
                .Where(x => x.entity_type == (int)EntityTypes.Chara)
                .Select(x => (Charas)x.id)
        );

        (
            IEnumerable<DbPlayerDragonData> newDragons,
            IEnumerable<DbPlayerDragonReliability> newReliability
        ) repositoryDragonOutput = await this.unitRepository.AddDragons(
            accountId,
            summonResult
                .Where(x => x.entity_type == (int)EntityTypes.Dragon)
                .Select(x => (Dragons)x.id)
        );

        UpdateDataList updateDataList = _summonService.GenerateUpdateData(
            repositoryCharaOuput,
            repositoryDragonOutput
        );

        IEnumerable<SummonReward> rewardList = _summonService.GenerateRewardList(
            summonResult,
            repositoryCharaOuput,
            repositoryDragonOutput
        );

        paymentHeldSetter(paymentCost);
        playerBannerData.SummonPoints += numSummons * summonPointMultiplier;
        playerBannerData.SummonCount += numSummons;

        List<BaseNewEntity> newEntities = rewardList
            .Where(x => x.is_new)
            .Select(x => new BaseNewEntity(x.entity_type, x.id))
            .ToList();

        EntityResult entityResult = new(new List<BaseNewEntity>(), newEntities);

        List<DbPlayerSummonHistory> historyEntries = new();

        int lastIndexOfRare5 = -1;
        int countOfRare5Char = 0;
        int countOfRare5Dragon = 0;
        int countOfRare4 = 0;
        int topRarity = 3;

        for (int i = 0; i < summonResult.Count; i++)
        {
            SimpleSummonReward summon = summonResult[i];
            topRarity = topRarity > summon.rarity ? topRarity : summon.rarity;
            EntityTypes entityType = (EntityTypes)summon.entity_type;
            if (summon.rarity == 5)
            {
                lastIndexOfRare5 = i;
                if (entityType == EntityTypes.Chara)
                {
                    countOfRare5Char++;
                }
                else if (entityType == EntityTypes.Dragon)
                {
                    countOfRare5Dragon++;
                }
            }
            if (summon.rarity == 4)
            {
                countOfRare4++;
            }

            bool isNew =
                newEntities.Find(
                    x => x.entity_type == summon.entity_type && x.entity_id == summon.id
                ) != null
                && !rewardList.Any(x => x.entity_type == summon.entity_type && x.id == summon.id);

            int dewGained = 0;
            if (!isNew && summon.entity_type == (int)EntityTypes.Chara)
            {
                dewGained = DewValueData.DupeSummon[summon.rarity];
                userData.DewPoint += dewGained;
            }

            //TODO: Get prize rank for this reward
            SummonPrizeRanks prizeRank = SummonPrizeRanks.None;

            historyEntries.Add(
                new DbPlayerSummonHistory()
                {
                    DeviceAccountId = accountId,
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
        {
            reversalIndex = -1;
        }

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
            switch (countOfRare4 + ((countOfRare5Char + countOfRare5Dragon) * 2))
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

        SummonRequestResponse response = new SummonRequestResponse(
            new SummonRequestResponseData(
                reversalIndex,
                new List<int>() { sageEffect, circleEffect },
                rewardList,
                new List<SummonPrize>(),
                new List<SummonTicket>(),
                playerBannerData.SummonPoints,
                new List<UserSummon>()
                {
                    new UserSummon(
                        bannerData.summon_id,
                        playerBannerData.SummonCount,
                        bannerData.campaign_type,
                        bannerData.free_count_rest,
                        bannerData.is_beginner_campaign,
                        bannerData.beginner_campaign_count_rest,
                        bannerData.consecution_campaign_count_rest
                    )
                },
                new SummonUpdateData()
                {
                    chara_list = updateDataList.chara_list,
                    dragon_list = updateDataList.dragon_list,
                    dragon_reliability_list = updateDataList.dragon_reliability_list,
                    summon_point_list = new()
                    {
                        new BannerIdSummonPoint(
                            bannerData.summon_id,
                            playerBannerData.SummonPoints,
                            playerBannerData.ConsecutionSummonPoints,
                            playerBannerData.ConsecutionSummonPointsMinDate,
                            playerBannerData.ConsecutionSummonPointsMaxDate
                        )
                    },
                    unit_story_list = new(),
                    user_data = SavefileUserDataFactory.Create(userData)
                },
                entityResult
            )
        );

        return this.Ok(response);
    }
}
