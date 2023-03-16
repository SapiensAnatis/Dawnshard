using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IMatchingService
{
    Task<IEnumerable<RoomList>> GetRoomList();
}
