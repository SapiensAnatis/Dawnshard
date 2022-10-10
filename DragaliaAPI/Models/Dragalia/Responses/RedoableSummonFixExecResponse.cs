using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Savefile;
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

public record EntityResult(List<Entity> converted_entity_list, List<Entity> new_get_entity_list);

public static class RedoableSummonFixExecFactory
{
    public static RedoableSummonFixExecData CreateData(
        List<SummonEntity> cachedSummonResult,
        List<Entity> convertedEntities,
        List<Entity> newEntities,
        SavefileUserData userData
    )
    {
        return new RedoableSummonFixExecData(
            new UserRedoableSummonData(0, cachedSummonResult),
            new UpdateDataList(userData),
            new EntityResult(convertedEntities, newEntities)
        );
    }
}
