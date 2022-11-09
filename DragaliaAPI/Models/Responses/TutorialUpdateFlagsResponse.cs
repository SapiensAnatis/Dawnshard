using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateFlagsResponse(TutorialUpdateFlagsData data)
    : BaseResponse<TutorialUpdateFlagsData>;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateFlagsData(
    List<int> tutorial_flag_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);
