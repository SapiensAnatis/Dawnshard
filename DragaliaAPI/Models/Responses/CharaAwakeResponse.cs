using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

public record CharaAwakeResponse(CharAwakeUpdateDataList data)
    : BaseResponse<CharAwakeUpdateDataList>;

[MessagePackObject(true)]
public class CharAwakeUpdateDataList : UpdateDataList
{
    public MissionNoticeData mission_notice { get; set; } = null!;
}
