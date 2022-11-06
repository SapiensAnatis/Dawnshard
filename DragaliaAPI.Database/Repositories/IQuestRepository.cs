using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository
{
    IQueryable<DbPlayerStoryState> GetQuestStoryList(string deviceAccountId);
    Task UpdateQuestStory(string deviceAccountId, int storyId, int state);
}
