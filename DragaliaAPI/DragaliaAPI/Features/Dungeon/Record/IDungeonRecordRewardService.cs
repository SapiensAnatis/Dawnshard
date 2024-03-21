using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordRewardService
{
    Task<(
        QuestMissionStatus MissionStatus,
        IEnumerable<AtgenFirstClearSet> FirstClearRewards
    )> ProcessQuestMissionCompletion(PlayRecord playRecord, DungeonSession session);

    Task<(IEnumerable<AtgenDropAll> DropList, int ManaDrop, int CoinDrop)> ProcessEnemyDrops(
        PlayRecord playRecord,
        DungeonSession session
    );

    Task<DungeonRecordRewardService.EventRewardData> ProcessEventRewards(
        PlayRecord playRecord,
        DungeonSession session
    );
}
