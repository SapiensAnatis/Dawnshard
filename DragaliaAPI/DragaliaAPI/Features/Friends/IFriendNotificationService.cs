using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

public interface IFriendNotificationService
{
    Task<FriendNotice?> GetFriendNotice(CancellationToken cancellationToken);
    Task<List<long>> GetNewFriendViewerIdList();
    Task<List<long>> GetNewFriendRequestViewerIdList();
}
