using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Features.CoOp;

public interface IPhotonStateApi
{
    Task<IEnumerable<ApiGame>> GetAllGames();
    Task<IEnumerable<ApiGame>> GetByQuestId(int questId);
    Task<ApiGame?> GetGameById(int id);
    Task<ApiGame?> GetGameByViewerId(long viewerId);
    Task<bool> GetIsHost(long viewerId);
}
