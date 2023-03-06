using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IStoryRepository
{
    /// <summary>
    /// Gets the quest/unit stories for a user.
    /// </summary>
    IQueryable<DbPlayerStoryState> Stories { get; }

    /// <summary>
    /// Gets the quests for a user.
    /// </summary>
    IQueryable<DbPlayerStoryState> QuestStories { get; }
    IQueryable<DbPlayerStoryState> UnitStories { get; }

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public IQueryable<DbPlayerStoryState> GetStoryList(string deviceAccountId);

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public Task<DbPlayerStoryState> GetOrCreateStory(
        string deviceAccountId,
        StoryTypes storyType,
        int storyId
    );

    Task<DbPlayerStoryState> GetOrCreateStory(StoryTypes storyType, int storyId);
}
