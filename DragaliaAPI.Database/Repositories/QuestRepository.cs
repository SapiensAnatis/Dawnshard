using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class QuestRepository : BaseRepository, IQuestRepository
{
    private readonly ApiContext apiContext;

    public QuestRepository(ApiContext apiContext) : base(apiContext)
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
    }

    public IQueryable<DbQuest> GetQuests(string deviceAccountId)
    {
        return this.apiContext.PlayerQuests.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public async Task UpdateQuestState(string deviceAccountId, int questId, int state)
    {
        DbQuest? questData = await apiContext.PlayerQuests.SingleOrDefaultAsync(
            x => x.DeviceAccountId == deviceAccountId && x.QuestId == questId
        );

        if (questData is null)
        {
            questData = new()
            {
                DeviceAccountId = deviceAccountId,
                QuestId = questId,
                State = (byte)state
            };
            apiContext.PlayerQuests.Add(questData);
        }

        questData.State = (byte)state;
    }

    public async Task CompleteQuest(string deviceAccountId, int questId)
    {
        DbQuest? questData = await apiContext.PlayerQuests.SingleOrDefaultAsync(
            x => x.DeviceAccountId == deviceAccountId && x.QuestId == questId
        );

        if (questData is null)
        {
            questData = new() { DeviceAccountId = deviceAccountId, QuestId = questId, };
            apiContext.PlayerQuests.Add(questData);
        }

        questData.State = 3;

        // TODO: need to track actual completion of missions in DungeonRecordController
        questData.IsMissionClear1 = true;
        questData.IsMissionClear2 = true;
        questData.IsMissionClear3 = true;
        questData.PlayCount++;
        questData.DailyPlayCount++;
        questData.WeeklyPlayCount++;
        questData.IsAppear = true;
        questData.BestClearTime = 4;
    }
}
