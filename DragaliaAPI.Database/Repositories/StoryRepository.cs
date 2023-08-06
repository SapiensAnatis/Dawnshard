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
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<StoryRepository> logger;

    public StoryRepository(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<StoryRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public IQueryable<DbPlayerStoryState> Stories =>
        this.apiContext.PlayerStoryState.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerStoryState> UnitStories =>
        this.Stories.Where(
            x => x.StoryType == StoryTypes.Chara || x.StoryType == StoryTypes.Dragon
        );

    public IQueryable<DbPlayerStoryState> QuestStories =>
        this.Stories.Where(x => x.StoryType == StoryTypes.Quest);

    public IQueryable<DbPlayerStoryState> DmodeStories =>
        this.Stories.Where(x => x.StoryType == StoryTypes.DungeonMode);

    public async Task<DbPlayerStoryState> GetOrCreateStory(StoryTypes storyType, int storyId)
    {
        DbPlayerStoryState? state = await apiContext.PlayerStoryState.FindAsync(
            this.playerIdentityService.AccountId,
            storyType,
            storyId
        );

        if (state is null)
        {
            this.logger.LogDebug(
                "Requested story id {id} with type {type} was not found, creating...",
                storyId,
                storyType
            );

            state = apiContext.PlayerStoryState
                .Add(
                    new DbPlayerStoryState()
                    {
                        DeviceAccountId = this.playerIdentityService.AccountId,
                        StoryId = storyId,
                        StoryType = storyType,
                        State = 0
                    }
                )
                .Entity;
        }

        return state;
    }

    public async Task<bool> HasReadQuestStory(int storyId) =>
        await this.QuestStories.AnyAsync(x => x.StoryId == storyId && x.State == StoryState.Read);
}
