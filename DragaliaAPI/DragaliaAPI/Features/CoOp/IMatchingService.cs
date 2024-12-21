using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.CoOp;

public interface IMatchingService
{
    Task<bool> GetIsHost();
    Task<MatchingGetRoomNameResponse?> GetRoomById(int id);
    Task<IEnumerable<RoomList>> GetRoomList();
    Task<IEnumerable<RoomList>> GetRoomList(int questId);
    Task<IEnumerable<Photon.Shared.Models.Player>> GetTeammates();
}
