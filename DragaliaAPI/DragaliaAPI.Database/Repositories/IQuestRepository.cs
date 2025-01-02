using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository
{
    [Obsolete("This entity has a global query filter, use ApiContext.PlayerQuests instead.")]
    IQueryable<DbQuest> Quests { get; }
    IQueryable<DbQuestEvent> QuestEvents { get; }
    IQueryable<DbQuestTreasureList> QuestTreasureList { get; }

    Task<DbQuest> GetQuestDataAsync(int questId);
    Task<DbQuestEvent> GetQuestEventAsync(int questEventId);
    Task DeleteQuests(IEnumerable<int> questIds);
}
