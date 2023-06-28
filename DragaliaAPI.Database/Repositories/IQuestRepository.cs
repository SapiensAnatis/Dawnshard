using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository
{
    IQueryable<DbQuest> Quests { get; }

    Task<DbQuest> CompleteQuest(int questId, float clearTime);
    Task UpdateQuestState(int questId, int state);
}
