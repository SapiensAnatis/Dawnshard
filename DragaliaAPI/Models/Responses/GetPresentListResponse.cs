using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record GetPresentListResponse(GetPresentListData data) : BaseResponse<GetPresentListData>;

[MessagePackObject(true)]
public record GetPresentListData(
    List<Present> present_list,
    List<Present> present_limit_list,
    PresentUpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record PresentUpdateDataList(
    PresentNotice present_notice,
    List<object> functional_maintanance_list
);

[MessagePackObject(true)]
public record PresentNotice(int present_count, int present_limit_count);

[MessagePackObject(true)]
public record Present(
    int present_id,
    int viewer_id,
    int master_id,
    //Later PresentStates
    int state,
    //Later EntityTypes
    int entity_type,
    int entity_id,
    int entity_quantity,
    int entity_level,
    int entity_limit_break_count,
    //UNKNOWN
    int entity_status_plus_count,
    int message_id,
    int message_param_value_1,
    int message_param_value_2,
    int message_param_value_3,
    int message_param_value_4,
    int extra_parameter_1,
    int extra_parameter_2,
    int extra_parameter_3,
    int extra_parameter_4,
    int extra_parameter_5,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset create_time,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset receive_limit_time
);

public static class GetPresentListFactory { }
