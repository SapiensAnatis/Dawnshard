using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository : IBaseRepository
{
    Task CompleteQuest(string deviceAccountId, int questId);
    IQueryable<DbQuest> GetQuests(string deviceAccountId);
    IQueryable<DbPlayerStoryState> GetStories(string deviceAccountId, StoryTypes type);
    Task UpdateQuestState(string deviceAccountId, int questId, int state);
    Task UpdateQuestStory(string deviceAccountId, int storyId, int state);
}
