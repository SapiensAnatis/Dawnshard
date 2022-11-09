using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

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
    List<BaseNewEntity> summon_exclude_unit_list,
    UpdateDataList update_data_list
);

public static class SummonExcludeGetListResponseFactory
{
    public static SummonExcludeGetListResponseData CreateData(
        List<BaseNewEntity> summonExcludeUnitList,
        UpdateDataList updateDataList
    )
    {
        return new SummonExcludeGetListResponseData(summonExcludeUnitList, updateDataList);
    }
}
