using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record RedoableSummonPreExecResponse(RedoableSummonPreExecData data)
    : BaseResponse<RedoableSummonPreExecData>;

[MessagePackObject(true)]
public record RedoableSummonPreExecData(
    UserRedoableSummonData user_redoable_summon_data,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record UserRedoableSummonData(
    int is_fixed_result,
    List<SummonEntity> redoable_summon_result_unit_list
);

public static class RedoableSummonPreExecFactory
{
    public static RedoableSummonPreExecData CreateData(List<SummonEntity> resultList)
    {
        return new(new(0, resultList), new());
    }
}
