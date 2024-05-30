using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Login;

public interface ILoginService
{
    Task<IEnumerable<AtgenLoginBonusList>> RewardLoginBonus();
    Task<IList<AtgenMonthlyWallReceiveList>> GetWallMonthlyReceiveList();
}
