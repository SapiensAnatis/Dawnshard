﻿using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

public record CharaAwakeResponse(CharAwakeUpdateDataList data)
    : BaseResponse<CharAwakeUpdateDataList>;

[MessagePackObject(true)]
public class CharAwakeUpdateDataList : UpdateDataList
{
    public MissionNoticeData mission_notice { get; set; } = null!;
}
