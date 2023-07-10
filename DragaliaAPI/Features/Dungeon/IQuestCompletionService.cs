using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Dungeon;

public interface IQuestCompletionService
{
    public Task<(
        IEnumerable<AtgenScoreMissionSuccessList> Missions,
        int Points
    )> CompleteQuestScoreMissions(int questId, PlayRecord record, DungeonSession session);

    public Task<QuestMissionStatus> CompleteQuestMissions(
        int questId,
        bool[] currentState,
        PlayRecord record,
        DungeonSession session
    );

    public Task<bool> IsQuestMissionCompleted(
        QuestCompleteType type,
        int completionValue,
        PlayRecord record,
        DungeonSession session
    );
}
