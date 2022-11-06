using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository
{
    IQueryable<DbPlayerStoryState> GetQuestStoryList(string deviceAccountId);
    Task UpdateQuestStory(string deviceAccountId, int storyId, int state);
}
