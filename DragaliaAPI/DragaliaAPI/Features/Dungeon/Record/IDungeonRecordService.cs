using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordService
{
    Task<IngameResultData> GenerateIngameResultData(
        string dungeonKey,
        PlayRecord playRecord,
        DungeonSession session
    );
}
