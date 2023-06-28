using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Services.Photon;

public interface IHeroParamService
{
    Task<IEnumerable<HeroParam>> GetHeroParam(long viewerId, int partySlot);
}
