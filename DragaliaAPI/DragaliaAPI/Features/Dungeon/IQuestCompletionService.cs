using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IQuestCompletionService
{
    public Task<(
        IEnumerable<AtgenScoreMissionSuccessList> Missions,
        int Points,
        int BoostedPoints
    )> CompleteQuestScoreMissions(DungeonSession session, PlayRecord record, double multiplier);

    Task<(IEnumerable<AtgenScoringEnemyPointList> Enemies, int Points)> CompleteEnemyScoreMissions(
        DungeonSession session,
        PlayRecord record
    );

    public Task<QuestMissionStatus> CompleteQuestMissions(
        DungeonSession session,
        bool[] currentState,
        PlayRecord record
    );

    public Task<IEnumerable<AtgenFirstClearSet>> GrantFirstClearRewards(int questId);
}
