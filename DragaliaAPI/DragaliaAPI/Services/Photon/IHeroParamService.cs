using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Services.Photon;

public interface IHeroParamService
{
    Task<List<HeroParam>> GetHeroParam(long viewerId, int partySlot);
}
