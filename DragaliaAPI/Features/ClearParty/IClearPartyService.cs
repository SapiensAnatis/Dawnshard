using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.ClearParty;

public interface IClearPartyService
{
    Task<IEnumerable<PartySettingList>> GetQuestClearParty(int questId, bool isMulti);
    void SetQuestClearParty(int questId, bool isMulti, IEnumerable<PartySettingList> party);
}
