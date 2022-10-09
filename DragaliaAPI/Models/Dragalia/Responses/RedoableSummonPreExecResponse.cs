using DragaliaAPI.Models.Dragalia.Responses.Common;
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
    public static RedoableSummonPreExecData CreateData()
    {
        List<SummonEntity> resultList = new();
        for (int i = 0; i < 50; i++)
            resultList.Add(new(entity_type: 7, id: 20050101, rarity: 5));

        return new(new(0, resultList), new(null));
    }
}
