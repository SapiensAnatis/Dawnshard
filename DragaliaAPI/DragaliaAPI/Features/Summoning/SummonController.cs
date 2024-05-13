using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.Summon;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Summoning;

[Route("summon")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class SummonController(
    SummonService summonService,
    SummonOddsService summonOddsService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    /// <summary>
    /// Returns excluded/excludable units for special banners
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("~/summon_exclude/get_list")]
    public DragaliaResult SummonExcludeGetList(SummonExcludeGetListRequest request)
    {
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

        return this.Ok(new SummonExcludeGetListResponse(excludableList));
    }

    [HttpPost]
    [Route("get_odds_data")]
    public async Task<DragaliaResult<SummonGetOddsDataResponse>> GetOddsData(
        SummonGetOddsDataRequest request
    )
    {
        OddsRate baseOddsRate = await summonOddsService.GetNormalOddsRate(request.SummonId);
        OddsRate? guaranteeOddsRate = await summonOddsService.GetGuaranteeOddsRate(
            request.SummonId
        );

        return new SummonGetOddsDataResponse(
            new OddsRateList(int.MaxValue, baseOddsRate, guaranteeOddsRate),
            new(null, null)
        );
    }

    [HttpPost]
    [Route("get_summon_history")]
    public async Task<DragaliaResult<SummonGetSummonHistoryResponse>> GetSummonHistory() =>
        new SummonGetSummonHistoryResponse(await summonService.GetSummonHistory());

    [HttpPost]
    [Route("get_summon_list")]
    public async Task<DragaliaResult<SummonGetSummonListResponse>> GetSummonList()
    {
        IList<SummonList> bannerList = await summonService.GetSummonList();
        IList<SummonTicketList> ticketList = await summonService.GetSummonTicketList();
        IList<SummonPointList> pointList = await summonService.GetSummonPointList();
        // csharpier-ignore-start
        return new SummonGetSummonListResponse()
        {
            SummonList = bannerList.Where(x => x.SummonType == SummonTypes.Normal),
            SummonTicketList = ticketList,
            SummonPointList = pointList,
            CampaignSummonList = [],
            CharaSsrSummonList = bannerList.Where(x => x.SummonType == SummonTypes.CharaSsr),
            DragonSsrSummonList = bannerList.Where(x => x.SummonType == SummonTypes.DragonSsr),
            CharaSsrUpdateSummonList = bannerList.Where(x => x.SummonType == SummonTypes.CharaSsrUpdate),
            DragonSsrUpdateSummonList = bannerList.Where(x => x.SummonType == SummonTypes.DragonSsrUpdate),
            CampaignSsrSummonList = bannerList.Where(x => x.SummonType == SummonTypes.CampaignSsr),
            PlatinumSummonList = bannerList.Where(x => x.SummonType == SummonTypes.Platinum),
            ExcludeSummonList = bannerList.Where(x => x.SummonType == SummonTypes.Exclude),
            CsSummonList = new()
            {
                SummonList = [],
                PlatinumSummonList = [],
                CampaignSummonList = [],
                CampaignSsrSummonList = [],
                ExcludeSummonList = [],
            },
        };
        // csharpier-ignore-end
    }

    [HttpPost]
    [Route("get_summon_point_trade")]
    public async Task<DragaliaResult> GetSummonPointTrade(
        SummonGetSummonPointTradeRequest request,
        CancellationToken cancellationToken
    )
    {
        SummonPointList? pointList = await summonService.GetSummonPointList(request.SummonId);
        IEnumerable<AtgenSummonPointTradeList>? tradeList = summonService.GetSummonPointTradeList(
            request.SummonId
        );

        if (pointList is null || tradeList is null)
        {
            return this.Code(
                ResultCode.SummonNotFound,
                $"Failed to get summon trade data for banner {request.SummonId}"
            );
        }

        // We need to save changes as we may have created a DbPlayerBannerData
        // in the process of fetching the SummonPointList.
        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(
            new SummonGetSummonPointTradeResponse(tradeList, [pointList], updateDataList, new())
        );
    }

    [HttpPost]
    [Route("request")]
    public async Task<DragaliaResult> RequestSummon(
        SummonRequestRequest summonRequest,
        CancellationToken cancellationToken
    )
    {
        int execCount = summonRequest.ExecCount > 0 ? summonRequest.ExecCount : 1;
        int summonCount = summonRequest.ExecType == SummonExecTypes.Tenfold ? 10 : execCount;

        SummonList? summonList = await summonService.GetSummonList(summonRequest.SummonId);

        if (summonList == null)
        {
            throw new DragaliaException(
                ResultCode.SummonNotFound,
                $"Failed to find a banner with ID {summonRequest.SummonId}"
            );
        }

        await summonService.ProcessSummonPayment(summonRequest, summonList);

        List<AtgenRedoableSummonResultUnitList> summonResult =
            await summonService.GenerateSummonResult(
                execCount,
                summonRequest.SummonId,
                summonRequest.ExecType
            );

        (
            IList<AtgenResultUnitList> resultUnitList,
            SummonService.SummonResultMetaInfo metaInfo,
            EntityResult entityResult
        ) = await summonService.CommitSummonResult(summonResult);

        foreach (AtgenResultUnitList result in resultUnitList)
        {
            summonService.AddSummonHistory(summonList, summonRequest, result);
        }

        UserSummonList userSummonList = await summonService.UpdateUserSummonInformation(
            summonList,
            summonCount
        );

        SummonEffect effect = SummonEffectHelper.CalculateEffect(metaInfo);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        SummonRequestResponse response =
            new(
                resultUnitList: resultUnitList,
                resultPrizeList: [],
                presageEffectList: [effect.SageEffect, effect.CircleEffect],
                reversalEffectIndex: effect.ReversalIndex,
                updateDataList: updateDataList,
                entityResult: entityResult,
                summonTicketList: await summonService.GetSummonTicketList(),
                resultSummonPoint: summonList.AddSummonPoint * summonCount,
                userSummonList: [userSummonList]
            );

        return this.Ok(response);
    }

    [HttpPost("summon_point_trade")]
    public async Task<DragaliaResult<SummonSummonPointTradeResponse>> SummonPointTrade(
        SummonSummonPointTradeRequest request,
        CancellationToken cancellationToken
    )
    {
        AtgenBuildEventRewardEntityList entity = await summonService.DoSummonPointTrade(
            request.SummonId,
            request.TradeId
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return new SummonSummonPointTradeResponse()
        {
            ExchangeEntityList = [entity],
            UpdateDataList = updateDataList,
        };
    }
}
