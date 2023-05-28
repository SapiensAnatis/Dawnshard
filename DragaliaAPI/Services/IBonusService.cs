using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IBonusService
{
    Task<FortBonusList> GetBonusList();
    Task<FortBonusList> GetBonusList(string deviceAccountId);
}
