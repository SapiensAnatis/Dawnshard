using DragaliaAPI.DTO;

namespace DragaliaAPI.Services;

public interface IBonusService
{
    Task<FortBonusList> GetBonusList();
    Task<AtgenEventBoost?> GetEventBoost(int eventId);
}
