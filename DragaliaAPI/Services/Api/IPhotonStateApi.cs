using DragaliaAPI.Photon.Dto.Game;

namespace DragaliaAPI.Services.Api;

public interface IPhotonStateApi
{
    Task<IEnumerable<ApiGame>> GetAllGames();
    Task<ApiGame?> GetGameById(int id);
}
