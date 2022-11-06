using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class QuestRepository : IQuestRepository
{
    private readonly ApiContext apiContext;

    public QuestRepository(ApiContext apiContext)
    {
        this.apiContext = apiContext;
    }

    public IQueryable<DbPlayerStoryState> GetQuestStoryList(string deviceAccountId)
    {
        return this.apiContext.PlayerStoryState.Where(
            x => x.DeviceAccountId == deviceAccountId && x.StoryType == StoryTypes.Quest
        );
    }

    public async Task UpdateQuestStory(string deviceAccountId, int storyId, int state)
    {
        DbPlayerStoryState? storyData = await apiContext.PlayerStoryState.SingleOrDefaultAsync(
            x => x.DeviceAccountId == deviceAccountId && x.StoryId == storyId
        );

        if (storyData is null)
        {
            storyData = new()
            {
                DeviceAccountId = deviceAccountId,
                StoryId = storyId,
                StoryType = StoryTypes.Quest
            };
            apiContext.PlayerStoryState.Add(storyData);
        }

        storyData.State = (byte)state;

        await apiContext.SaveChangesAsync();
    }
}
