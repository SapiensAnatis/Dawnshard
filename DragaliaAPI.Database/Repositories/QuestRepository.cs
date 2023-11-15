﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;

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
        this.apiContext
            .PlayerQuests
            .Where(x => x.DeviceAccountId == this.playerIdentityService.AccountId);

    public IQueryable<DbQuestEvent> QuestEvents =>
        this.apiContext
            .QuestEvents
            .Where(x => x.DeviceAccountId == this.playerIdentityService.AccountId);

    public IQueryable<DbQuestTreasureList> QuestTreasureList =>
        this.apiContext
            .QuestTreasureList
            .Where(x => x.DeviceAccountId == this.playerIdentityService.AccountId);

    private async Task<DbQuest?> FindQuestAsync(int questId)
    {
        return await apiContext.PlayerQuests.FindAsync(playerIdentityService.AccountId, questId);
    }

    public async Task<DbQuest> GetQuestDataAsync(int questId)
    {
        DbQuest? questData = await FindQuestAsync(questId);
        questData ??= this.apiContext
            .PlayerQuests
            .Add(
                new DbQuest
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
                    QuestId = questId
                }
            )
            .Entity;
        return questData;
    }

    private async Task<DbQuestEvent?> FindQuestEventAsync(int questEventId)
    {
        return await apiContext
            .QuestEvents
            .FindAsync(playerIdentityService.AccountId, questEventId);
    }

    public async Task<DbQuestEvent> GetQuestEventAsync(int questEventId)
    {
        return await FindQuestEventAsync(questEventId)
            ?? apiContext
                .QuestEvents
                .Add(
                    new DbQuestEvent
                    {
                        DeviceAccountId = playerIdentityService.AccountId,
                        QuestEventId = questEventId
                    }
                )
                .Entity;
    }
}
