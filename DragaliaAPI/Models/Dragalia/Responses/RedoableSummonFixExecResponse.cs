using DragaliaAPI.Models.Dragalia.Responses.Common;
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

public record EntityResult(
    List<SummonEntity> converted_entity_list,
    List<SummonEntity> new_get_entity_list
);

public static class RedoableSummonFixExecFactory
{
    public static RedoableSummonFixExecData CreateData()
    {
        //TODO: get cached resultList
        List<SummonEntity> resultList = new List<SummonEntity>();

        //TODO: create both from non-/existance in PlayerCharaData/PlayerDragonReliability
        List<SummonEntity> convertedUnitList = new List<SummonEntity>();
        List<SummonEntity> newUnitList = new List<SummonEntity>();

        return new RedoableSummonFixExecData(
            new UserRedoableSummonData(0, resultList),
            new UpdateDataList(null),
            new EntityResult(convertedUnitList, newUnitList)
        );
    }
}
