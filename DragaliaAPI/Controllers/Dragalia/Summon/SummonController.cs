using DragaliaAPI.Models.Database.Savefile;
using System.Text.Json.Serialization;
using System.Text.Json;
using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Services;
using DragaliaAPI.Models.Dragalia.Responses.Summon;
using static DragaliaAPI.Models.Dragalia.Responses.Summon.SummonHistoryFactory;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Pipelines.Sockets.Unofficial;
using DragaliaAPI.Models.Nintendo;
using System.Collections.Immutable;
using MessagePack;
using Microsoft.AspNetCore.Identity;
using DragaliaAPI.Models.Database;
using System;

namespace DragaliaAPI.Controllers.Dragalia.Summon;

[Route("summon")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class SummonController : ControllerBase
{
    private readonly IApiRepository _apiRepository;
    private readonly ISessionService _sessionService;
    private readonly ISummonService _summonService;
    private readonly ISavefileWriteService _saveService;

    public SummonController(
        IApiRepository apiRepository,
        ISessionService sessionService,
        ISummonService summonService,
        ISavefileWriteService saveService
    )
    {
        _apiRepository = apiRepository;
        _sessionService = sessionService;
        _summonService = summonService;
        _saveService = saveService;
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
        DbPlayerUserData userData = await _apiRepository.GetPlayerInfo(accountId).FirstAsync();
        //TODO Replace DummyData with real exludes from BannerInfo
        List<BaseNewEntity> excludableList = new List<BaseNewEntity>();
        foreach (Charas c in Enum.GetValues<Charas>())
        {
            excludableList.Add(new BaseNewEntity((int)EntityTypes.Chara, (int)c));
        }
        foreach (Dragons d in Enum.GetValues<Dragons>())
        {
            excludableList.Add(new BaseNewEntity((int)EntityTypes.Dragon, (int)d));
        }
        return Ok(
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
        DbPlayerUserData userData = await _apiRepository.GetPlayerInfo(accountId).FirstAsync();
        //TODO Replace Dummy data with oddscalculation
        return Ok(
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
        DbPlayerUserData userData = await _apiRepository.GetPlayerInfo(accountId).FirstAsync();
        List<SummonHistory> dbList = (await _apiRepository.GetSummonHistory(accountId))
            .Select(x => SummonHistoryFactory.Create(x))
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
        DbPlayerBannerData dummy = await _apiRepository.GetPlayerBannerData(accountId, 1020203);
        data.summon_point_list.Add(
            new(
                dummy.SummonBannerId,
                dummy.SummonPoints,
                dummy.ConsecutionSummonPoints,
                dummy.ConsecutionSummonPointsMinDate,
                dummy.ConsecutionSummonPointsMaxDate
            )
        );
        dummy = await _apiRepository.GetPlayerBannerData(accountId, 1110003);
        data.summon_point_list.Add(
            new(
                dummy.SummonBannerId,
                dummy.SummonPoints,
                dummy.ConsecutionSummonPoints,
                dummy.ConsecutionSummonPointsMinDate,
                dummy.ConsecutionSummonPointsMaxDate
            )
        );
        DbPlayerUserData userData = await _apiRepository.GetPlayerInfo(accountId).FirstAsync();
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
        DbPlayerUserData userData = await _apiRepository.GetPlayerInfo(accountId).FirstAsync();
        //TODO maybe throw BadRequest on bad banner id, for now generate empty data if not exists
        DbPlayerBannerData playerBannerData = await _apiRepository.GetPlayerBannerData(
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

    //TODO: Fully implement and REFACTOR
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

        DbPlayerBannerData playerBannerData = await _apiRepository.GetPlayerBannerData(
            accountId,
            bannerData.summon_id
        );

        DbPlayerUserData userData = await _apiRepository.GetPlayerInfo(accountId).FirstAsync();

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
        UpdateDataList commitResult = await _saveService.CommitSummonResult(
            summonResult,
            accountId,
            false
        );

        paymentHeldSetter(paymentCost);
        playerBannerData.SummonPoints += numSummons * summonPointMultiplier;
        playerBannerData.SummonCount += numSummons;

        List<BaseNewEntity> newEntities = (commitResult.chara_list ?? new())
            .Select(x => new BaseNewEntity((int)EntityTypes.Chara, (int)x.chara_id))
            .Concat(
                (commitResult.dragon_reliability_list ?? new()).Select(
                    x => new BaseNewEntity((int)EntityTypes.Dragon, (int)x.dragon_id)
                )
            )
            .ToList();

        EntityResult entityResult = new(new List<BaseNewEntity>(), newEntities);

        List<SummonReward> rewardList = new List<SummonReward>();
        List<DbPlayerSummonHistory> historyEntries = new List<DbPlayerSummonHistory>();

        int lastIndexOfRare5 = -1;
        int countOfRare5Char = 0;
        int countOfRare5Dragon = 0;
        int countOfRare4 = 0;
        for (int i = 0; i < summonResult.Count; i++)
        {
            SimpleSummonReward summon = summonResult[i];
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
                && rewardList.Find(x => x.entity_type == summon.entity_type && x.id == summon.id)
                    == null;

            int dewGained = 0;
            if (!isNew && summon.entity_type == (int)EntityTypes.Chara)
            {
                dewGained = DewValueData.DupeSummon[summon.rarity];
                userData.DewPoint += dewGained;
            }
            rewardList.Add(
                new SummonReward(summon.entity_type, summon.id, summon.rarity, isNew, dewGained)
            );

            //TODO: Get prize rank for this reward
            SummonPrizeRanks prizeRank = SummonPrizeRanks.None;

            historyEntries.Add(
                new DbPlayerSummonHistory()
                {
                    DeviceAccountId = accountId,
                    BannerId = bannerData.summon_id,
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
                    EntityAtkPlusCount = 0,
                    SummonPrizeRank = prizeRank,
                    SummonPointGet = summonPointMultiplier,
                    DewPointGet = dewGained
                }
            );
        }

        int saved = await _saveService.CreateSummonHistory(historyEntries);

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
                new() { sageEffect, circleEffect },
                rewardList,
                new(),
                new(),
                playerBannerData.SummonPoints,
                new()
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
                    chara_list = commitResult.chara_list,
                    dragon_list = commitResult.dragon_list,
                    dragon_reliability_list = commitResult.dragon_reliability_list,
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
        return Ok(response);
    }
}
