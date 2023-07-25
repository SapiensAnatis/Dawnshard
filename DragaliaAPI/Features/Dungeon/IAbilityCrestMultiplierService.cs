using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IAbilityCrestMultiplierService
{
    Task<double> GetFacilityEventMultiplier(
        IEnumerable<PartySettingList> partySettingList,
        int eventId
    );
}
