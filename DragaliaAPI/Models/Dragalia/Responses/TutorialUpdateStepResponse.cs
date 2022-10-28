using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateStepResponse(TutorialUpdateStepData data)
    : BaseResponse<TutorialUpdateStepData>;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateStepData(int step, object update_data_list);
