using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Login;

public interface ILoginBonusService
{
    Task<IEnumerable<AtgenLoginBonusList>> RewardLoginBonus();
}
