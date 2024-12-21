using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Fort;

public interface IBonusService
{
    Task<FortBonusList> GetBonusList();
    Task<AtgenEventBoost?> GetEventBoost(int eventId);
}
