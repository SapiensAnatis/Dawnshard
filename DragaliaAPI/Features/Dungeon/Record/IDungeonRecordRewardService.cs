using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordRewardService
{
    Task<IngameResultData> ProcessQuestRewards(
        IngameResultData resultData,
        DungeonSession session,
        PlayRecord playRecord,
        DbQuest questData
    );
}
