using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

/// <summary>
/// Separate service for friend functionality, most of which is used by <see cref="Shared.UpdateDataService"/>.
/// Split out from <see cref="FriendService"/> to reduce the risk of circular dependencies.
/// </summary>
public class FriendNotificationService(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : IFriendNotificationService
{
    public async Task<FriendNotice?> GetFriendNotice(CancellationToken cancellationToken)
    {
        // TODO - consider caching this as it is called for every UpdateDataList

        int newFriendCount = await this.GetNewFriendsQuery().CountAsync(cancellationToken);
        int newFriendRequestCount = await apiContext
            .PlayerFriendRequests.Where(x =>
                x.ToPlayerViewerId == playerIdentityService.ViewerId && x.IsNew
            )
            .CountAsync(cancellationToken);

        if (newFriendCount == 0 && newFriendRequestCount == 0)
        {
            return null;
        }

        return new() { ApplyNewCount = newFriendRequestCount, FriendNewCount = newFriendCount };
    }

    public async Task<List<long>> GetNewFriendViewerIdList()
    {
        return await this.GetNewFriendsQuery().Select(x => x.ViewerId).ToListAsync();
    }

    public async Task<List<long>> GetNewFriendRequestViewerIdList()
    {
        return await apiContext
            .PlayerFriendRequests.IgnoreQueryFilters()
            .Where(x => x.ToPlayerViewerId == playerIdentityService.ViewerId && x.IsNew)
            .Select(x => x.FromPlayerViewerId)
            .ToListAsync();
    }

    private IQueryable<DbPlayer> GetNewFriendsQuery()
    {
        return apiContext
            .PlayerFriendshipPlayers.IgnoreQueryFilters()
            .Where(x => x.PlayerViewerId == playerIdentityService.ViewerId && x.IsNew)
            .Select(x => x.Friendship)
            .SelectMany(x => x!.Players)
            .Where(x => x.ViewerId != playerIdentityService.ViewerId);
    }
}
