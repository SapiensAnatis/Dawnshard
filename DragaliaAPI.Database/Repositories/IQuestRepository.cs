using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository : IBaseRepository
{
    Task CompleteQuest(string deviceAccountId, int questId);
    IQueryable<DbQuest> GetQuests(string deviceAccountId);
    IQueryable<DbPlayerStoryState> GetQuestStoryList(string deviceAccountId);
    Task UpdateQuestState(string deviceAccountId, int questId, int state);
    Task UpdateQuestStory(string deviceAccountId, int storyId, int state);
}
