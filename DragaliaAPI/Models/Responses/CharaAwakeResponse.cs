using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

public record CharaAwakeResponse(CharAwakeUpdateDataList data)
    : BaseResponse<CharAwakeUpdateDataList>;

[MessagePackObject(true)]
public class CharAwakeUpdateDataList : UpdateDataList
{
    public MissionNoticeData mission_notice { get; set; } = null!;
}
