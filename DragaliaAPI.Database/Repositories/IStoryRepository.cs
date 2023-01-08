using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IStoryRepository : IBaseRepository
{
    public IQueryable<DbPlayerStoryState> GetStoryList(string deviceAccountId);
    public Task<DbPlayerStoryState> GetOrCreateStory(
        string deviceAccountId,
        StoryTypes storyType,
        int storyId
    );
    Task UpdateStory(string deviceAccountId, StoryTypes storyType, int storyId, byte newState);
}
