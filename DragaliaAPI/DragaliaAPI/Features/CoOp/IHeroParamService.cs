using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Features.CoOp;

public interface IHeroParamService
{
    Task<List<HeroParam>> GetHeroParam(long viewerId, int partySlot);
}
