using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Fort;

public interface IBonusService
{
    Task<FortBonusList> GetBonusList(CancellationToken cancellationToken = default);
    Task<FortBonusList> GetBonusList(long viewerId, CancellationToken cancellationToken = default);
    Task<AtgenEventBoost?> GetEventBoost(int eventId);
}
