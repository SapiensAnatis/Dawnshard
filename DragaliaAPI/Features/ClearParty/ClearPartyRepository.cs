using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.ClearParty;

public class ClearPartyRepository : IClearPartyRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public ClearPartyRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbQuestClearPartyUnit> QuestClearPartyUnits =>
        this.apiContext.QuestClearPartyUnits.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbQuestClearPartyUnit> GetQuestClearParty(int questId, bool isMulti)
    {
        return this.QuestClearPartyUnits.Where(x => x.QuestId == questId && x.IsMulti == isMulti)
            .OrderBy(x => x.UnitNo);
    }

    public void SetQuestClearParty(
        int questId,
        bool isMulti,
        IEnumerable<DbQuestClearPartyUnit> units
    )
    {
        Debug.Assert(units.All(u => u.QuestId == questId));
        Debug.Assert(units.All(u => u.IsMulti == isMulti));

        this.apiContext.QuestClearPartyUnits.RemoveRange(this.GetQuestClearParty(questId, isMulti));
        this.apiContext.QuestClearPartyUnits.AddRange(units);
    }
}
