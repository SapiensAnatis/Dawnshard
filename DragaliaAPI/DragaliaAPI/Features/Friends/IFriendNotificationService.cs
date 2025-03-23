using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

public interface IFriendNotificationService
{
    Task<FriendNotice?> GetFriendNotice(CancellationToken cancellationToken);
    Task<List<ulong>> GetNewFriendViewerIdList();
    Task<List<ulong>> GetNewFriendRequestViewerIdList();
}
