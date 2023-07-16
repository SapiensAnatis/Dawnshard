using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.ClearParty;

public interface IClearPartyRepository
{
    IQueryable<DbQuestClearPartyUnit> QuestClearPartyUnits { get; }

    void SetQuestClearParty(int questId, bool isMulti, IEnumerable<DbQuestClearPartyUnit> units);
    IQueryable<DbQuestClearPartyUnit> GetQuestClearParty(int questId, bool isMulti);
}
