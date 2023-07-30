using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordHelperService
{
    Task<IngameResultData> ProcessHelperDataMulti(IngameResultData resultData);
    Task<IngameResultData> ProcessHelperDataSolo(
        IngameResultData resultData,
        ulong? supportViewerId
    );
}
