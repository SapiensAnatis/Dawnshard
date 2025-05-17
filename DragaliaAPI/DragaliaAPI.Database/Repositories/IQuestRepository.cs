using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository
{
    Task<DbQuest> GetQuestDataAsync(int questId);
    Task<DbQuestEvent> GetQuestEventAsync(int questEventId);
    Task DeleteQuests(IEnumerable<int> questIds);
}
