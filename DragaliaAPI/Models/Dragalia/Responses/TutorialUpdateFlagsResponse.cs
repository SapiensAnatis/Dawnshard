using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateFlagsResponse(TutorialUpdateFlagsData data)
    : BaseResponse<TutorialUpdateFlagsData>;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateFlagsData(
    List<int> tutorial_flag_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);
