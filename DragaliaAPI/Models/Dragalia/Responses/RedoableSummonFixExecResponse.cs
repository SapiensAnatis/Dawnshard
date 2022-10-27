using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record RedoableSummonFixExecResponse(RedoableSummonFixExecData data)
    : BaseResponse<RedoableSummonFixExecData>;

[MessagePackObject(true)]
public record RedoableSummonFixExecData(
    UserRedoableSummonData user_redoable_summon_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record EntityResult(
    IEnumerable<BaseNewEntity> converted_entity_list,
    IEnumerable<BaseNewEntity> new_get_entity_list
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
