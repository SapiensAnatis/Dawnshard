using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

public record CharaBuildupManaResponse(CharBuildupManaUpdateDataList data)
    : BaseResponse<CharBuildupManaUpdateDataList>;

[MessagePackObject(true)]
public class CharBuildupManaUpdateDataList : UpdateDataList
{
    public PartyPowerData party_power_data { get; set; } = null!;
    public MissionNoticeData mission_notice { get; set; } = null!;
}
