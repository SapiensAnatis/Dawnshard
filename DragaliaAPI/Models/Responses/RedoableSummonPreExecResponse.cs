using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

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
    List<SimpleSummonReward> redoable_summon_result_unit_list
);

public static class RedoableSummonPreExecFactory
{
    public static RedoableSummonPreExecData CreateData(List<SimpleSummonReward> resultList)
    {
        return new(new(0, resultList), new());
    }
}
