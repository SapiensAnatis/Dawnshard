using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class StoryRepository : BaseRepository, IStoryRepository
{
    private readonly ApiContext apiContext;

    public StoryRepository(ApiContext apiContext) : base(apiContext)
    {
        this.apiContext = apiContext;
    }

    public IQueryable<DbPlayerStoryState> GetStoryList(string deviceAccountId)
    {
        return this.apiContext.PlayerStoryState.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public async Task<DbPlayerStoryState> GetOrCreateStory(
        string deviceAccountId,
        StoryTypes storyType,
        int storyId
    )
    {
        return await apiContext.PlayerStoryState.SingleOrDefaultAsync(
                x => x.DeviceAccountId == deviceAccountId && x.StoryId == storyId
            )
            ?? apiContext.PlayerStoryState
                .Add(
                    new DbPlayerStoryState()
                    {
                        DeviceAccountId = deviceAccountId,
                        StoryId = storyId,
                        StoryType = storyType,
                        State = 0
                    }
                )
                .Entity;
    }

    public async Task UpdateStory(
        string deviceAccountId,
        StoryTypes storyType,
        int storyId,
        byte newState
    )
    {
        // TODO: I don't think we should really add them if they don't exist as that may indicate not-unlocked.
        // Need to research how to determine if story is accessible and implement API-side validation for this.
        // Fine for now to trust the client.

        DbPlayerStoryState story =
            await apiContext.PlayerStoryState.FindAsync(deviceAccountId, storyType, storyId)
            ?? apiContext.PlayerStoryState
                .Add(
                    new DbPlayerStoryState()
                    {
                        DeviceAccountId = deviceAccountId,
                        StoryId = storyId,
                        StoryType = storyType,
                        State = newState
                    }
                )
                .Entity;

        story.State = newState;
    }
}
