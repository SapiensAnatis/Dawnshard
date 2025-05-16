using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
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
                friend.Player!.Helper,
                LastHelperUseDate = friend.Player.Helper!.UseDates.FirstOrDefault(x =>
                    x.PlayerViewerId == playerIdentityService.ViewerId
                ),
                LastQuestClearDate = friend
                    .Player.QuestList.OrderByDescending(x => x.LastDailyResetTime)
                    .First()
                    .LastDailyResetTime,
            };

        List<UserSupportList> friends = (
            await friendsQuery
                .Where(x =>
                    x.LastHelperUseDate == null
                    || x.LastHelperUseDate.UseDate < x.LastQuestClearDate
                    || x.LastHelperUseDate.UseDate < lastResetDate
                )
                .Select(x => x.Helper)
                .ProjectToHelperProjection()
                .IgnoreQueryFilters()
                .ToListAsyncEF(cancellationToken)
        )
            .Select(x => x.MapToUserSupportList())
            .ToList();

        List<UserSupportList> nonFriends = (
            await apiContext
                .Players.Where(x => x.ViewerId != playerIdentityService.ViewerId)
                .OrderByDescending(x => x.UserData!.LastLoginTime)
                .Where(x => x.Helper != null)
                .Where(x =>
                    // Don't suggest people who are already your friends
                    !apiContext.PlayerFriendships.Any(y =>
                        y.PlayerFriendshipPlayers.Any(z =>
                            z.PlayerViewerId == playerIdentityService.ViewerId
                        ) && y.PlayerFriendshipPlayers.Any(z => z.PlayerViewerId == x.ViewerId)
                    )
                )
                .Select(x => new
                {
                    x.Helper,
                    LastHelperUseDate = x.Helper!.UseDates.FirstOrDefault(y =>
                        y.PlayerViewerId == playerIdentityService.ViewerId
                    ),
                    LastQuestClearDate = x
                        .QuestList.OrderByDescending(y => y.LastDailyResetTime)
                        .First()
                        .LastDailyResetTime,
                })
                .Take(10)
                .Where(x =>
                    x.LastHelperUseDate == null
                    || x.LastHelperUseDate.UseDate < x.LastQuestClearDate
                    || x.LastHelperUseDate.UseDate < lastResetDate
                )
                .Select(x => x.Helper!)
                .ProjectToHelperProjection()
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .ToListAsyncEF(cancellationToken)
        ).Select(x => x.MapToUserSupportList()).ToList();

        List<UserSupportList> merged = [.. friends, .. nonFriends];
        List<AtgenSupportUserDetailList> details =
        [
            .. friends.Select(x => new AtgenSupportUserDetailList()
            {
                ViewerId = x.ViewerId,
                GettableManaPoint = HelperConstants.HelperFriendRewardMana,
                IsFriend = true,
            }),
            .. nonFriends.Select(x => new AtgenSupportUserDetailList()
            {
                ViewerId = x.ViewerId,
                GettableManaPoint = HelperConstants.HelperRewardMana,
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
        HelperProjection? projection = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            apiContext
                .Players.Where(x => x.ViewerId == helperViewerId)
                .Select(x => x.Helper!)
                .ProjectToHelperProjection()
                .AsSplitQuery()
                .IgnoreQueryFilters(),
            cancellationToken
        );

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
            .AsSplitQuery()
            .FirstOrDefaultAsyncEF(cancellationToken);

        if (projection is null)
        {
            return null;
        }

        IQueryable<DbPlayer> friends = apiContext
            .Players.Where(x => x.ViewerId == playerIdentityService.ViewerId)
            .SelectMany(x => x.Friendships)
            .SelectMany(x => x.Players)
            .IgnoreQueryFilters();

        bool isFriend = await friends.AnyAsyncEF(
            x => x.ViewerId == helperViewerId,
            cancellationToken
        );

        bool hasSentFriendRequest = await apiContext.PlayerFriendRequests.AnyAsyncEF(
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

        await apiContext
            .PlayerHelperUseDates.ToLinqToDBTable()
            .InsertOrUpdateAsync(
                () =>
                    new DbPlayerHelperUseDate()
                    {
                        HelperViewerId = helperViewerId,
                        PlayerViewerId = playerIdentityService.ViewerId,
                        UseDate = timeProvider.GetUtcNow(),
                    },
                existing => new DbPlayerHelperUseDate()
                {
                    HelperViewerId = existing.HelperViewerId,
                    PlayerViewerId = existing.PlayerViewerId,
                    UseDate = timeProvider.GetUtcNow(),
                },
                cancellationToken
            );

        bool helperIsFriend = await friendService
            .GetFriendsQuery()
            .AnyAsyncEF(x => x.ViewerId == helperViewerId, cancellationToken);

        IQueryable<DbPlayerHelper> targetHelper = apiContext
            .PlayerHelpers.Where(x => x.ViewerId == helperViewerId)
            .IgnoreQueryFilters();

        int rewardRowsUpdated;

        if (helperIsFriend)
        {
            rewardRowsUpdated = await targetHelper.ExecuteUpdateAsync(
                e =>
                    e.SetProperty(
                        p => p.HelperFriendRewardCount,
                        p => p.HelperFriendRewardCount + 1
                    ),
                cancellationToken
            );
        }
        else
        {
            // csharpier-ignore
            rewardRowsUpdated = await targetHelper.ExecuteUpdateAsync(
                e =>
                    e.SetProperty(
                        p => p.HelperRewardCount,
                        p => p.HelperRewardCount + 1
                    ),
                cancellationToken
            );
        }

        if (rewardRowsUpdated != 1)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Failed to grant helper rewards"
            );
        }

        await transaction.CommitAsync(cancellationToken);
    }
}
