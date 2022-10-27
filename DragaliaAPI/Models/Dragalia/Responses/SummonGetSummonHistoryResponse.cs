using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;
using static DragaliaAPI.Models.Dragalia.Responses.Summon.SummonHistoryFactory;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record SummonGetSummonHistoryResponse(SummonGetSummonHistoryResponseData data)
    : BaseResponse<SummonGetSummonHistoryResponseData>;

/// <summary>
///
/// </summary>
/// <param name="summon_history_list"></param>
/// <param name="update_data_list"></param>
[MessagePackObject(true)]
public record SummonGetSummonHistoryResponseData(
    List<SummonHistory> summon_history_list,
    UpdateDataList update_data_list
);

public static class SummonGetSummonHistoryResponseFactory
{
    public static SummonGetSummonHistoryResponseData CreateData(
        List<SummonHistory> summonHistoryList,
        UpdateDataList updateDataList
    )
    {
        return new SummonGetSummonHistoryResponseData(summonHistoryList, updateDataList);
    }
}
