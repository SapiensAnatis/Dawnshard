using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository
{
    IQueryable<DbQuest> Quests { get; }
    IQueryable<DbQuestEvent> QuestEvents { get; }

    Task<DbQuest> GetQuestDataAsync(int questId);
    Task<DbQuestEvent> GetQuestEventAsync(int questEventId);
    Task DeleteQuests(IEnumerable<int> questIds);
}
