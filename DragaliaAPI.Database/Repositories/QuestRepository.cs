﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class QuestRepository : IQuestRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public QuestRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbQuest> Quests =>
        this.apiContext.PlayerQuests.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task UpdateQuestState(int questId, int state)
    {
        DbQuest? questData = await apiContext.PlayerQuests.FindAsync(
            this.playerIdentityService.AccountId,
            questId
        );

        if (questData is null)
        {
            questData = new()
            {
                DeviceAccountId = this.playerIdentityService.AccountId,
                QuestId = questId,
                State = (byte)state
            };
            apiContext.PlayerQuests.Add(questData);
        }

        questData.State = (byte)state;
    }

    public async Task<DbQuest> GetQuestDataAsync(int questId)
    {
        return await this.Quests.SingleAsync(x => x.QuestId == questId);
    }

    public async Task<DbQuest> CompleteQuest(int questId, float clearTime)
    {
        DbQuest? questData = await apiContext.PlayerQuests.SingleOrDefaultAsync(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId && x.QuestId == questId
        );

        if (questData is null)
        {
            questData = new()
            {
                DeviceAccountId = this.playerIdentityService.AccountId,
                QuestId = questId,
            };
            apiContext.PlayerQuests.Add(questData);
        }

        questData.State = 3;

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
