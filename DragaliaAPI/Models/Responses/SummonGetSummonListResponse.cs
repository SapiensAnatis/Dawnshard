using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record SummonGetSummonListResponse(SummonGetSummonListResponseData data)
    : BaseResponse<SummonGetSummonListResponseData>;

[MessagePackObject(true)]
public record SummonGetSummonListResponseData(
    List<BannerData>? summon_list,
    List<BannerData>? campaign_summon_list,
    List<BannerData>? chara_ssr_summon_list,
    List<BannerData>? dragon_ssr_summon_list,
    List<BannerData>? chara_ssr_update_summon_list,
    List<BannerData>? dragon_ssr_update_summon_list,
    List<BannerData>? campaign_ssr_summon_list,
    List<BannerData>? platinum_summon_list,
    List<BannerData>? exclude_summon_list,
    BannerList cs_summon_list,
    List<SummonTicket> summon_ticket_list,
    List<BannerIdSummonPoint> summon_point_list,
    UpdateDataList update_data_list
)
    : BannerList(
        summon_list,
        campaign_summon_list,
        chara_ssr_summon_list,
        dragon_ssr_summon_list,
        chara_ssr_update_summon_list,
        dragon_ssr_update_summon_list,
        campaign_ssr_summon_list,
        platinum_summon_list,
        exclude_summon_list
    );

/// UNKNOWN: Are those all possible lists?
/// <summary>
/// List of types of banners, mostly just for the top small banner image
/// </summary>
/// <param name="summon_list"></param>
/// <param name="campaign_summon_list"></param>
/// <param name="chara_ssr_summon_list"></param>
/// <param name="dragon_ssr_summon_list"></param>
/// <param name="chara_ssr_update_summon_list"></param>
/// <param name="dragon_ssr_update_summon_list"></param>
/// <param name="campaign_ssr_summon_list"></param>
/// <param name="platinum_summon_list"></param>
/// <param name="exclude_summon_list">Summon Banners to hide</param>
[MessagePackObject(true)]
public record BannerList(
    List<BannerData>? summon_list,
    List<BannerData>? campaign_summon_list,
    List<BannerData>? chara_ssr_summon_list,
    List<BannerData>? dragon_ssr_summon_list,
    List<BannerData>? chara_ssr_update_summon_list,
    List<BannerData>? dragon_ssr_update_summon_list,
    List<BannerData>? campaign_ssr_summon_list,
    List<BannerData>? platinum_summon_list,
    List<BannerData>? exclude_summon_list
);

/// UNKNOWN: params: priority, summon_type, status, daily(unsure), campaign_type, [x]_rest
/// <summary>
/// Banner Data<br/>
/// This is composed from static banner data and DB saved player-banner data
/// </summary>
/// <param name="summon_id">Banner Id</param>
/// <param name="priority">Unknown</param>
/// <param name="summon_type">Unknown, maybe for special banners like platinum only banners</param>
/// <param name="single_crystal">1x summon Wyrmite cost (Negative numbers won't allow summons, 0 for default)</param>
/// <param name="single_diamond">Client uses <see cref="single_crystal"/> for displaying both wyrmite and diamantium cost<br/>Most likely 1x summon Diamantium cost (Negative numbers won't allow summons, 0 for default)</param>
/// <param name="multi_crystal">10x summon Wyrmite cost (Negative numbers won't allow summons, 0 for default)</param>
/// <param name="multi_diamond">Client uses <see cref="multi_crystal"/> for displaying both wyrmite and diamantium cost<br/>Most likely 10x summon Diamantium cost (Negative numbers won't allow summons, 0 for default)</param>
/// <param name="limited_crystal">Unknown: Presumably Wyrmite cost of the limited 1x summon button but it never existed</param>
/// <param name="limited_diamond">Diamantium cost of the limited 1x summon button</param>
/// <param name="add_summon_point">Summon points for a 1x Wyrmite summon</param>
/// <param name="add_summon_point_stone">Summon points for a 1x Diamantium summon</param>
/// <param name="exchange_summon_point">Summon point cost for sparking, the client doesn't seem to care though</param>
/// <param name="status">Unknown function, maybe just active = 1, inactive = 0 but no change in normal banner</param>
/// <param name="commence_date">Banner start date</param>
/// <param name="complete_date">Banner end date</param>
/// <param name="daily_count">Currently used summons for the daily discounted diamantium summon</param>
/// <param name="daily_limit">Total limit for the daily discounted diamantium summon</param>
/// <param name="total_limit">Total amount of summons limit(seems ignored for normal banners)</param>
/// <param name="total_count">Current total amount of summons(seems ignored for normal banners)</param>
/// <param name="campaign_type">Unknown, maybe used for </param>
/// <param name="free_count_rest">Most likely free summons for certain banner/campaign types</param>
/// <param name="is_beginner_campaign">If this banner is part of the beginner campaign</param>
/// <param name="beginner_campaign_count_rest">Begginer banner has a free tenfold available(only if <see cref="is_beginner_campaign"/> is set)</param>
/// <param name="consecution_campaign_count_rest">Unknown</param>
[MessagePackObject(true)]
public record BannerData(
    int summon_id,
    int? priority,
    BannerTypes summon_type,
    int single_crystal,
    int single_diamond,
    int multi_crystal,
    int multi_diamond,
    int limited_crystal,
    int limited_diamond,
    int add_summon_point,
    int add_summon_point_stone,
    int exchange_summon_point,
    int status,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset commence_date,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset complete_date,
    int daily_count,
    int daily_limit,
    int total_limit,
    int total_count,
    SummonCampaignTypes campaign_type,
    int free_count_rest,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_beginner_campaign,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool beginner_campaign_count_rest,
    int consecution_campaign_count_rest
);

public static class SummonGetSummonListResponseFactory
{
    public static SummonGetSummonListResponseData CreateData(
        BannerList banners,
        BannerList csBanners,
        List<SummonTicket> ticketList,
        List<BannerIdSummonPoint> pointList,
        UpdateDataList updateDataList
    )
    {
        return new SummonGetSummonListResponseData(
            banners.summon_list,
            banners.campaign_summon_list,
            banners.chara_ssr_summon_list,
            banners.dragon_ssr_summon_list,
            banners.chara_ssr_update_summon_list,
            banners.dragon_ssr_update_summon_list,
            banners.campaign_ssr_summon_list,
            banners.platinum_summon_list,
            banners.exclude_summon_list,
            csBanners,
            ticketList,
            pointList,
            updateDataList
        );
    }

    public static SummonGetSummonListResponseData CreateDummyData()
    {
        return new SummonGetSummonListResponseData(
            new()
            {
                new BannerData(
                    1020203,
                    null,
                    BannerTypes.Normal,
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
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow.AddDays(7),
                    0,
                    1,
                    0,
                    0,
                    SummonCampaignTypes.Normal,
                    0,
                    true,
                    true,
                    0
                ),
                new BannerData(
                    1110003,
                    null,
                    BannerTypes.DiamantiumMulti,
                    0,
                    0,
                    0,
                    1500,
                    0,
                    0,
                    0,
                    0,
                    300,
                    1,
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow.AddDays(7),
                    0,
                    5,
                    0,
                    0,
                    SummonCampaignTypes.Normal,
                    0,
                    false,
                    false,
                    0
                )
            },
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new() { },
            new(),
            new(new(), new(), new(), new(), new(), new(), new(), new(), new()),
            new(),
            new(),
            new()
        );
    }
}
