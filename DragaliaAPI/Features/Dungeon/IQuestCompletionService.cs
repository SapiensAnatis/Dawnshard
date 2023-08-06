using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Dungeon;

public interface IQuestCompletionService
{
    public Task<(
        IEnumerable<AtgenScoreMissionSuccessList> Missions,
        int Points,
        int BoostedPoints
    )> CompleteQuestScoreMissions(DungeonSession session, PlayRecord record, double multiplier);

    public Task<QuestMissionStatus> CompleteQuestMissions(
        DungeonSession session,
        bool[] currentState,
        PlayRecord record
    );

    public Task<bool> IsQuestMissionCompleted(
        QuestCompleteType type,
        int completionValue,
        PlayRecord record,
        DungeonSession session
    );

    public Task<IEnumerable<AtgenFirstClearSet>> GrantFirstClearRewards(int questId);
}
