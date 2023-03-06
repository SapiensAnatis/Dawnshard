using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class StoryRepository : IStoryRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;
    private readonly ILogger<StoryRepository> logger;

    public StoryRepository(
        ApiContext apiContext,
        IPlayerDetailsService playerDetailsService,
        ILogger<StoryRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
        this.logger = logger;
    }

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public IQueryable<DbPlayerStoryState> GetStoryList(string deviceAccountId)
    {
        return this.apiContext.PlayerStoryState.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public async Task<DbPlayerStoryState> GetOrCreateStory(
        string deviceAccountId,
        StoryTypes storyType,
        int storyId
    )
    {
        DbPlayerStoryState? state = await apiContext.PlayerStoryState.FindAsync(
            deviceAccountId,
            storyType,
            storyId
        );

        if (state is null)
        {
            this.logger.LogInformation(
                "Requested story id {id} with type {type} was not found, creating...",
                storyId,
                storyType
            );

            state = apiContext.PlayerStoryState
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

        return state;
    }

    public async Task<DbPlayerStoryState> GetOrCreateStory(StoryTypes storyType, int storyId)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return await this.GetOrCreateStory(this.playerDetailsService.AccountId, storyType, storyId);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public IQueryable<DbPlayerStoryState> Stories =>
        this.apiContext.PlayerStoryState.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public IQueryable<DbPlayerStoryState> UnitStories =>
        this.Stories.Where(
            x => x.StoryType == StoryTypes.Chara || x.StoryType == StoryTypes.Dragon
        );

    public IQueryable<DbPlayerStoryState> QuestStories =>
        this.Stories.Where(x => x.StoryType == StoryTypes.Quest);
}
