using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Features.Login;

public interface ILoginBonusService
{
    Task<IEnumerable<AtgenLoginBonusList>> RewardLoginBonus();
}
