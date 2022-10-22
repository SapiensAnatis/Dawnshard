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

    [HttpPost]
    [Route("~/summon_exclude/get_list")]
    public async Task<DragaliaResult> SummonExcludeGetList(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] int bannerId
    )
    {
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbPlayerUserData userData = await _apiRepository.GetPlayerInfo(accountId).FirstAsync();
        //TODO Replace DummyData with real exludes
        return Ok(
            new SummonExcludeGetListResponse(
                SummonExcludeGetListResponseFactory.CreateData(
                    new(),
                    new UpdateDataList()
                    {
                        user_data = SavefileUserDataFactory.Create(
                            userData,
                            TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag).ToList()
                        )
                    }
                )
            )
        );
    }

    [HttpPost]
    [Route("get_odds_data")]
    public async Task<DragaliaResult> GetOddsData(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] int bannerId
    )
    {
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbPlayerUserData userData = await _apiRepository.GetPlayerInfo(accountId).FirstAsync();
        //TODO Replace Dummy data with oddscalculation
        return Ok(
            new SummonGetOddsDataResponse(
                SummonGetOddsDataResponseFactory.CreateDummyData(
                    SavefileUserDataFactory.Create(
                        userData,
                        TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag).ToList()
                    )
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
                    new UpdateDataList()
                    {
                        user_data = SavefileUserDataFactory.Create(
                            userData,
                            TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag).ToList()
                        )
                    }
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
                user_data = SavefileUserDataFactory.Create(
                    userData,
                    TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag).ToList()
                )
            }
        };
        return Ok(new SummonGetSummonListResponse(data));
    }

    [HttpPost]
    [Route("get_summon_point_trade")]
    public async Task<DragaliaResult> GetSummonPointTrade(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] int bannerId
    )
    {
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
                SavefileUserDataFactory.Create(
                    userData,
                    TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag).ToList()
                )
            )
        );
        return Ok(response);
    }

    //TODO: Fully implement and refactor
    [HttpPost]
    [Route("request")]
    public async Task<DragaliaResult> RequestSummon(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] SummonRequest summonRequest
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
            0,
            0,
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
            summonRequest.exec_type == SummonExecTypes.Tenfold ? 10 : summonRequest.exec_count;
        int summonPointMultiplier = bannerData.add_summon_point;
        int paymentHeld = 0;
        Action<int> paymentHeldSetter = reduction => { };
        int paymentCost = 1;
        switch (summonRequest.payment_type)
        {
            case PaymentTypes.Wyrmite:
                paymentHeld = userData.Crystal;
                paymentHeldSetter = reduction => userData.Crystal -= reduction;
                paymentCost =
                    summonRequest.exec_type == SummonExecTypes.Tenfold
                        ? bannerData.multi_crystal
                        : bannerData.single_crystal * summonRequest.exec_count;
                break;
            case PaymentTypes.SummonVoucher:
                paymentCost =
                    summonRequest.exec_type == SummonExecTypes.Tenfold
                        ? 1
                        : summonRequest.exec_count * summonRequest.exec_count;
                break;
            case PaymentTypes.Free:
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

        Models.Dragalia.Responses.EntityResult entityResult =
            new(new List<BaseNewEntity>(), newEntities);

        int summonEffect = summonResult.Select(x => x.rarity).Max();

        List<SummonReward> rewardList = new List<SummonReward>();
        List<DbPlayerSummonHistory> historyEntries = new List<DbPlayerSummonHistory>();
        foreach (SimpleSummonReward summon in summonResult)
        {
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
                    SummonPrizeRank = 1,
                    SummonPointGet = summonPointMultiplier,
                    DewPointGet = dewGained
                }
            );
        }

        int saved = await _saveService.CreateSummonHistory(historyEntries);

        SummonRequestResponse response = new SummonRequestResponse(
            new SummonRequestResponseData(
                0,
                new() { summonEffect - 2, summonEffect },
                rewardList,
                new(),
                new(),
                playerBannerData.SummonPoints,
                new()
                {
                    new UserSummon(
                        bannerData.summon_id,
                        playerBannerData.SummonCount + numSummons,
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
                    user_data = SavefileUserDataFactory.Create(
                        userData,
                        TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag).ToList()
                    )
                },
                entityResult
            )
        );
        ;
        return Ok(response);
    }
}
