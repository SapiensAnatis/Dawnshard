using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;
using static DragaliaAPI.Models.Dragalia.Responses.Summon.SummonHistoryFactory;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record SummonExcludeGetListResponse(SummonExcludeGetListResponseData data)
    : BaseResponse<SummonExcludeGetListResponseData>;

/// <summary>
///
/// </summary>
/// <param name="summon_exclude_unit_list"></param>
/// <param name="update_data_list"></param>
[MessagePackObject(true)]
public record SummonExcludeGetListResponseData(
    List<EntityResult> summon_exclude_unit_list,
    UpdateDataList update_data_list
);

public static class SummonExcludeGetListResponseFactory
{
    public static SummonExcludeGetListResponseData CreateData(
        List<EntityResult> summonExcludeUnitList,
        UpdateDataList updateDataList
    )
    {
        return new SummonExcludeGetListResponseData(summonExcludeUnitList, updateDataList);
    }
}
