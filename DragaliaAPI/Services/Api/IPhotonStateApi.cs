using DragaliaAPI.Photon.Dto;

namespace DragaliaAPI.Services.Api;

public interface IPhotonStateApi
{
    Task<IEnumerable<StoredGame>> GetAllGames();
}
