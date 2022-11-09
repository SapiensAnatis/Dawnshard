using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record RedoableSummonFixExecResponse(RedoableSummonFixExecData data)
    : BaseResponse<RedoableSummonFixExecData>;

[MessagePackObject(true)]
public record RedoableSummonFixExecData(
    UserRedoableSummonData user_redoable_summon_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

public static class RedoableSummonFixExecFactory
{
    public static RedoableSummonFixExecData CreateData(
        List<SimpleSummonReward> cachedSummonResult,
        UpdateDataList updateDataList
    )
    {
        return new RedoableSummonFixExecData(
            new UserRedoableSummonData(1, cachedSummonResult),
            updateDataList,
            new EntityResult(
                new List<BaseNewEntity>(),
                cachedSummonResult.Select(x => new BaseNewEntity(x.entity_type, x.id))
            )
        );
    }
}
