using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IAbilityCrestMultiplierService
{
    Task<(double Material, double Point)> GetEventMultiplier(
        IEnumerable<PartySettingList> partySettingList,
        int eventId
    );
}
