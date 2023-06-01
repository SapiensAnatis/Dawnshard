using DragaliaAPI.Photon.Dto;

namespace DragaliaAPI.Services.Photon;

public interface IHeroParamService
{
    Task<IEnumerable<HeroParam>> GetHeroParam(long viewerId, IEnumerable<int> partySlots);
}
