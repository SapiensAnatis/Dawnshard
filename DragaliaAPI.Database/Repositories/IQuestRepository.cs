using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository
{
    Task<DbQuest> CompleteQuest(string deviceAccountId, int questId, float clearTime);
    IQueryable<DbQuest> GetQuests(string deviceAccountId);
    Task UpdateQuestState(string deviceAccountId, int questId, int state);
    IQueryable<DbQuest> Quests { get; }
}
