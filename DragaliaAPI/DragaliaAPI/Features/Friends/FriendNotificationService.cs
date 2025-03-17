using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

/// <summary>
/// Separate service for friend functionality required by <see cref="Shared.UpdateDataService"/>.
/// Split out from <see cref="FriendService"/> to reduce the risk of circular dependencies.
/// </summary>
public class FriendNotificationService(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
)
{
    public async Task<int> GetNewFriendCount()
    {
        return await this.GetNewFriendsQuery().CountAsync();
    }

    public async Task<List<long>> GetNewFriendViewerIdList()
    {
        return await this.GetNewFriendsQuery().Select(x => x.ViewerId).ToListAsync();
    }

    public async Task<int> GetNewFriendRequestCount()
    {
        return await apiContext
            .PlayerFriendRequests.IgnoreQueryFilters()
            .Where(x => x.ToPlayerViewerId == playerIdentityService.ViewerId && x.IsNew)
            .CountAsync();
    }

    private IQueryable<DbPlayer> GetNewFriendsQuery()
    {
        return apiContext
            .PlayerFriendshipPlayers.IgnoreQueryFilters()
            .Where(x => x.PlayerViewerId == playerIdentityService.ViewerId && x.IsNew)
            .Select(x => x.Friendship)
            .SelectMany(x => x!.Players);
    }
}
