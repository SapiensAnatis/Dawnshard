using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record SummonRequestResponse(SummonRequestResponseData data)
    : BaseResponse<SummonRequestResponseData>;

/// <summary>
/// Data for a SummonRequest response
/// </summary>
/// <param name="reversal_effect_index">Index in <see cref="result_prize_list"/> to do a fakeout on (rarity 4 orb in sky turns to rarity 5 upon landing)</param>
/// <param name="presage_effect_list">Probably the summon effect (doves, rainbow, fafnirs, etc)</param>
/// <param name="result_unit_list">List of rolled units</param>
/// <param name="result_prize_list">List of rolled prizes from prize summons</param>
/// <param name="summon_ticket_list">Probably list of used summon tickets<br/>List because of summons with multiple singles?</param>
/// <param name="result_summon_point">Summon points for sparking</param>
/// <param name="user_summon_list">Updated summon banner data</param>
/// <param name="update_data_list">Updated user data</param>
/// <param name="entity_result">List of converted and new entities</param>
[MessagePackObject(true)]
public record SummonRequestResponseData(
    int reversal_effect_index,
    IEnumerable<int> presage_effect_list,
    IEnumerable<SummonReward> result_unit_list,
    IEnumerable<SummonPrize> result_prize_list,
    IEnumerable<SummonTicket> summon_ticket_list,
    int result_summon_point,
    IEnumerable<UserSummon> user_summon_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

/// <summary>
/// Updated Summon Banner Data
/// </summary>
/// <param name="summon_id">Id of the summon banner</param>
/// <param name="summon_count">Total Amount of times a player pressed summon on a banner<br/>
/// <b>Not the same as amount of this summon</b></param>
/// <param name="campaign_type">Type of banner.<br/> 0 for default, other values unknown</param>
/// UNKNOWN param: free_count_rest
/// <param name="free_count_rest">Unknown</param>
/// <param name="is_beginner_campaign">Flag for if this banner is part of the beginner campaign</param>
/// <param name="beginner_campaign_count_rest">Free 10x availabe if is beginner campaign</param>
/// UNKNOWN param: consecution_campaign_count_rest
/// <param name="consecution_campaign_count_rest">Unknown</param>
[MessagePackObject(true)]
public record UserSummon(
    int summon_id,
    int summon_count,
    SummonCampaignTypes campaign_type,
    int free_count_rest,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_beginner_campaign,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool beginner_campaign_count_rest,
    int consecution_campaign_count_rest
);
