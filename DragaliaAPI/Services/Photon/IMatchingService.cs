using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services.Photon;

public interface IMatchingService
{
    Task<IEnumerable<RoomList>> GetRoomList();
}
