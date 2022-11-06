using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record SummonGetSummonPointTradeResponse(SummonGetSummonPointTradeResponseResponseData data)
    : BaseResponse<SummonGetSummonPointTradeResponseResponseData>;

/// <summary>
/// Data for a SummonRequest response
/// </summary>
/// <param name="summon_point_trade_list">List of tradable entities</param>
/// <param name="summon_point_list">List of summon points</param>
/// <param name="entity_result">List of converted and new entities</param>
[MessagePackObject(true)]
public record SummonGetSummonPointTradeResponseResponseData(
    List<TradableEntity> summon_point_trade_list,
    List<BannerIdSummonPoint> summon_point_list,
    UpdateDataList update_data_list
);

/// <summary>
///
/// </summary>
/// <param name="summon_point_id">Actually bannerId the points can be used on</param>
/// <param name="summon_point">Amount of points</param>
/// <param name="cs_summon_point">Probably points from previous banner if this is a followup banner</param>
/// <param name="cs_point_term_min_date">Probably earliest date when <paramref name="cs_summon_point"/> are usable</param>
/// <param name="cs_point_term_max_date">Probably latest date when <paramref name="cs_summon_point"/> are usable</param>
[MessagePackObject(true)]
public record BannerIdSummonPoint(
    int summon_point_id,
    int summon_point,
    int cs_summon_point,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset cs_point_term_min_date,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset cs_point_term_max_date
);

public static class SummonGetSummonPointTradeResponseResponseFactory
{
    public static SummonGetSummonPointTradeResponseResponseData CreateData(
        List<TradableEntity> tradableUnits,
        BannerIdSummonPoint summonPointInfo,
        UserData userData
    )
    {
        return new(tradableUnits, new() { summonPointInfo }, new() { user_data = userData });
    }
}
