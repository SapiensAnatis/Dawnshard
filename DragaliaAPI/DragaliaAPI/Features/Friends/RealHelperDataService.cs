using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

internal sealed class RealHelperDataService(
    ApiContext apiContext,
    TimeProvider timeProvider,
    FriendService friendService,
    IPlayerIdentityService playerIdentityService
) : IHelperDataService
{
    public async Task<QuestGetSupportUserListResponse> GetHelperList(
        CancellationToken cancellationToken
    )
    {
        DateTimeOffset lastResetDate = timeProvider.GetLastDailyReset();

        var friendsQuery =
            from friendship in apiContext.PlayerFriendshipPlayers
            where friendship.PlayerViewerId == playerIdentityService.ViewerId
            join friend in apiContext.PlayerFriendshipPlayers.Where(x =>
                x.PlayerViewerId != playerIdentityService.ViewerId
            )
                on friendship.FriendshipId equals friend.FriendshipId
            select new
            {
                friend.Player.Helper,
                friendship.LastHelperUseDate,
                LastQuestClearDate = friend
                    .Player.QuestList.OrderByDescending(x => x.LastDailyResetTime)
                    .First()
                    .LastDailyResetTime, // does this even get translated?
            };

        List<UserSupportList> friends = (
            await friendsQuery
                .Where(x =>
                    x.LastHelperUseDate < x.LastQuestClearDate
                    || x.LastHelperUseDate < lastResetDate
                )
                .Select(x => x.Helper)
                .ProjectToHelperProjection()
                .ToListAsync(cancellationToken)
        )
            .Select(x => x.MapToUserSupportList())
            .ToList();

        List<UserSupportList> nonFriends = await friendService.GetRecommendedFriendsList(
            cancellationToken
        );

        List<UserSupportList> merged = [.. friends, .. nonFriends];
        List<AtgenSupportUserDetailList> details =
        [
            .. friends.Select(x => new AtgenSupportUserDetailList()
            {
                ViewerId = x.ViewerId,
                GettableManaPoint = 50,
                IsFriend = true,
            }),
            .. nonFriends.Select(x => new AtgenSupportUserDetailList()
            {
                ViewerId = x.ViewerId,
                GettableManaPoint = 25,
                IsFriend = false,
            }),
        ];

        return new QuestGetSupportUserListResponse()
        {
            SupportUserList = merged,
            SupportUserDetailList = details,
        };
    }

    public Task<UserSupportList?> GetHelper(
        long helperViewerId,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task<AtgenSupportUserDataDetail?> GetHelperDataDetail(
        long helperViewerId,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task UseHelper(long helperViewerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
