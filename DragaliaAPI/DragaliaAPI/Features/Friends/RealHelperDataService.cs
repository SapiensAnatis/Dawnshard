using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DragaliaAPI.Features.Friends;

internal sealed class RealHelperDataService(
    ApiContext apiContext,
    TimeProvider timeProvider,
    FriendService friendService,
    IBonusService bonusService,
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
                .IgnoreQueryFilters()
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

    public async Task<UserSupportList?> GetHelper(
        long helperViewerId,
        CancellationToken cancellationToken
    )
    {
        HelperProjection? projection = await apiContext
            .Players.Where(x => x.ViewerId == helperViewerId)
            .Select(x => x.Helper!)
            .ProjectToHelperProjection()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(cancellationToken);

        if (projection is null)
        {
            return null;
        }

        return projection.MapToUserSupportList();
    }

    public async Task<AtgenSupportUserDataDetail?> GetHelperDataDetail(
        long helperViewerId,
        CancellationToken cancellationToken
    )
    {
        HelperProjection? projection = await apiContext
            .Players.Where(x => x.ViewerId == helperViewerId)
            .Select(x => x.Helper!)
            .ProjectToHelperProjection()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(cancellationToken);

        if (projection is null)
        {
            return null;
        }

        IQueryable<DbPlayer> friends = apiContext
            .Players.Where(x => x.ViewerId == playerIdentityService.ViewerId)
            .SelectMany(x => x.Friendships)
            .SelectMany(x => x.Players)
            .IgnoreQueryFilters();

        bool isFriend = await friends.AnyAsync(
            x => x.ViewerId == helperViewerId,
            cancellationToken
        );

        bool hasSentFriendRequest = await apiContext.PlayerFriendRequests.AnyAsync(
            x =>
                x.FromPlayerViewerId == playerIdentityService.ViewerId
                && x.ToPlayerViewerId == helperViewerId,
            cancellationToken
        );

        FortBonusList fortBonusList = await bonusService.GetBonusList(
            helperViewerId,
            cancellationToken
        );

        return new AtgenSupportUserDataDetail()
        {
            UserSupportData = projection.MapToUserSupportList(),
            ManaCirclePieceIdList = projection.ManaCirclePieceIdList,
            DragonReliabilityLevel = projection.ReliabilityLevel ?? 0,
            IsFriend = isFriend,
            ApplySendStatus = hasSentFriendRequest ? 1 : 0,
            FortBonusList = fortBonusList,
        };
    }

    public async Task UseHelper(long helperViewerId, CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction =
            await apiContext.Database.BeginTransactionAsync(cancellationToken);

        int rowsUpdated = await apiContext
            .PlayerFriendshipPlayers.Where(x =>
                x.PlayerViewerId == playerIdentityService.ViewerId
                && x.Friendship!.Players.Any(y => y.ViewerId == helperViewerId)
            )
            .ExecuteUpdateAsync(
                e => e.SetProperty(p => p.LastHelperUseDate, DateTimeOffset.UtcNow),
                cancellationToken
            );

        if (rowsUpdated != 1)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Failed to update helper use date"
            );
        }

        await transaction.CommitAsync(cancellationToken);
    }
}
