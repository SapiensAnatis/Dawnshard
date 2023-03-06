using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class QuestRepository : IQuestRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;

    public QuestRepository(ApiContext apiContext, IPlayerDetailsService playerDetailsService)
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
    }

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public IQueryable<DbQuest> GetQuests(string deviceAccountId)
    {
        return this.apiContext.PlayerQuests.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbQuest> Quests =>
        this.apiContext.PlayerQuests.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public async Task UpdateQuestState(string deviceAccountId, int questId, int state)
    {
        DbQuest? questData = await apiContext.PlayerQuests.FindAsync(deviceAccountId, questId);

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

    public async Task<DbQuest> CompleteQuest(string deviceAccountId, int questId, float clearTime)
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

        if (clearTime < questData.BestClearTime || questData.BestClearTime == -1.0f)
        {
            questData.BestClearTime = clearTime;
        }

        return questData;
    }
}
