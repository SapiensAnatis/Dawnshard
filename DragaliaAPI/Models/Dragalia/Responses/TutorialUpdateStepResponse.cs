﻿using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateStepResponse(TutorialUpdateStepData data)
    : BaseResponse<TutorialUpdateStepData>;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateStepData(int step, UpdateDataList update_data_list);
