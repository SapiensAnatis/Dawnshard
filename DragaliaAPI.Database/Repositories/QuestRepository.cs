using DragaliaAPI.Database.Entities;
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
        this.apiContext.PlayerQuests.Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbQuestEvent> QuestEvents =>
        this.apiContext.QuestEvents.Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbQuestTreasureList> QuestTreasureList =>
        this.apiContext.QuestTreasureList.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    private async Task<DbQuest?> FindQuestAsync(int questId)
    {
        return await apiContext.PlayerQuests.FindAsync(playerIdentityService.ViewerId, questId);
    }

    public async Task<DbQuest> GetQuestDataAsync(int questId)
    {
        DbQuest? questData = await FindQuestAsync(questId);
        questData ??= this.apiContext.PlayerQuests.Add(
            new DbQuest { ViewerId = this.playerIdentityService.ViewerId, QuestId = questId }
        ).Entity;
        return questData;
    }

    private async Task<DbQuestEvent?> FindQuestEventAsync(int questEventId)
    {
        return await apiContext.QuestEvents.FindAsync(playerIdentityService.ViewerId, questEventId);
    }

    public async Task<DbQuestEvent> GetQuestEventAsync(int questEventId)
    {
        return await FindQuestEventAsync(questEventId)
            ?? apiContext
                .QuestEvents.Add(
                    new DbQuestEvent
                    {
                        ViewerId = playerIdentityService.ViewerId,
                        QuestEventId = questEventId
                    }
                )
                .Entity;
    }

    public async Task DeleteQuests(IEnumerable<int> questIds)
    {
        List<DbQuest> questEntities = await this.Quests.Where(x => questIds.Contains(x.QuestId))
            .ToListAsync();
        this.apiContext.PlayerQuests.RemoveRange(questEntities);
    }
}
