using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateStepResponse(TutorialUpdateStepData data)
    : BaseResponse<TutorialUpdateStepData>;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateStepData(int step, UpdateDataList update_data_list);
