using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.ClearParty;

public interface IClearPartyRepository
{
    IQueryable<DbQuestClearPartyUnit> QuestClearPartyUnits { get; }
    Task<IEnumerable<DbQuestClearPartyUnit>> GetQuestClearPartyAsync(int questId, bool isMulti);
    void SetQuestClearParty(int questId, bool isMulti, IEnumerable<DbQuestClearPartyUnit> units);
}
