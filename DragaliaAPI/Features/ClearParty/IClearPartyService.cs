using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.ClearParty;

public interface IClearPartyService
{
    Task<(
        IEnumerable<PartySettingList> PartyList,
        IEnumerable<AtgenLostUnitList> LostUnitList
    )> GetQuestClearParty(int questId, bool isMulti);
    Task SetQuestClearParty(int questId, bool isMulti, IEnumerable<PartySettingList> party);
}
