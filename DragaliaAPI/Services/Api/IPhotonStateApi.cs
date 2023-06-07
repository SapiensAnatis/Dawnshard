using DragaliaAPI.Photon.Dto.Game;

namespace DragaliaAPI.Services.Api;

public interface IPhotonStateApi
{
    Task<IEnumerable<Game>> GetAllGames();
}
