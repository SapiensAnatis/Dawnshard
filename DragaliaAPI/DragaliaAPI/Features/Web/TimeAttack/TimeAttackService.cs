using DragaliaAPI.Database;
using DragaliaAPI.Features.Web.TimeAttack.Models;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Web.TimeAttack;

internal sealed class TimeAttackService(ApiContext apiContext)
{
    public async Task<List<TimeAttackQuest>> GetQuests()
    {
        List<int> uniqueQuestIds = await apiContext
            .TimeAttackClears.Select(x => x.QuestId)
            .Distinct()
            .ToListAsync();

        return uniqueQuestIds
            .Select(questId => new TimeAttackQuest()
            {
                Id = questId,
                IsCoop = MasterAsset.QuestData.GetValueOrDefault(questId)?.CanPlayCoOp ?? false,
            })
            .ToList();
    }

    public async Task<List<TimeAttackRanking>> GetRankings(int questId)
    {
        var list = await apiContext
            .TimeAttackPlayers.Where(x => x.Clear.QuestId == questId)
            .Select(x => new
            {
                x.ViewerId,
                x.GameId,
                x.Clear.Time,
            })
            .GroupBy(x => x.ViewerId)
            .Select(g => g.OrderBy(x => x.Time).First())
            .ToListAsync();

        return [];
    }
}
