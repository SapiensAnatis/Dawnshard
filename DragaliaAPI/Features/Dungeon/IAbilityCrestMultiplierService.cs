using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IAbilityCrestMultiplierService
{
    Task<(double Material, double Point)> GetFacilityEventMultiplier(
        IEnumerable<PartySettingList> partySettingList,
        int eventId
    );
}
