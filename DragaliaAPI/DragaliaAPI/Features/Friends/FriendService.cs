using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DragaliaAPI.Features.Friends;

internal sealed partial class FriendService(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<FriendService> logger
)
{
    public async Task<int> GetFriendCount()
    {
        return await this.GetFriendsQuery().CountAsync();
    }

    public async Task<int> GetNewApplyCount()
    {
        return await apiContext.PlayerFriendRequests.CountAsync(x =>
            x.ToPlayerViewerId == playerIdentityService.ViewerId && x.IsNew
        );
    }

    public async Task<bool> CheckIfFriendshipExists(
        long otherPlayerId,
        CancellationToken cancellationToken = default
    )
    {
        return await this.GetFriendsQuery()
            .AnyAsync(x => x.ViewerId == otherPlayerId, cancellationToken);
    }

    public async Task<List<UserSupportList>> GetFriendList()
    {
        IQueryable<HelperProjection> helperQuery = this.GetFriendsQuery()
            .Select(x => x.Helper!)
            .ProjectToHelperProjection()
            .AsSplitQuery();

        List<HelperProjection> mergedHelpers = await helperQuery.ToListAsync();

        return mergedHelpers.Select(x => x.MapToUserSupportList()).ToList();
    }

    public async Task ResetNewFriends(
        IEnumerable<long> targetIdList,
        CancellationToken cancellationToken
    )
    {
        IList<long> targetIdListEnumerated = targetIdList as IList<long> ?? targetIdList.ToList();

        IDbContextTransaction transaction = await apiContext.Database.BeginTransactionAsync(
            cancellationToken
        );

        IQueryable<DbPlayerFriendshipPlayer> playerFriendships =
            apiContext.PlayerFriendshipPlayers.Where(x =>
                x.PlayerViewerId == playerIdentityService.ViewerId && x.IsNew
            );

        IQueryable<DbPlayerFriendshipPlayer> matchingIdFriendships =
            apiContext.PlayerFriendshipPlayers.Where(x =>
                targetIdListEnumerated.Contains(x.PlayerViewerId)
            );

        IQueryable<DbPlayerFriendshipPlayer> friendshipsToUpdate = playerFriendships.Join(
            matchingIdFriendships,
            x => x.FriendshipId,
            x => x.FriendshipId,
            (currentPlayerFriendship, otherPlayerFriendship) => currentPlayerFriendship
        );

        int rowsAffected = await friendshipsToUpdate.ExecuteUpdateAsync(
            e => e.SetProperty(x => x.IsNew, false),
            cancellationToken
        );

        if (rowsAffected != targetIdListEnumerated.Count)
        {
            throw new DragaliaException(
                ResultCode.ModelUpdateTargetNotFound,
                $"Invalid friend reset new: expected {targetIdListEnumerated.Count} rows to be updated, got {rowsAffected}"
            );
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task ResetNewRequests(CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction =
            await apiContext.Database.BeginTransactionAsync(cancellationToken);

        IQueryable<DbPlayerFriendRequest> matchingRequests = apiContext.PlayerFriendRequests.Where(
            x => x.ToPlayerViewerId == playerIdentityService.ViewerId
        );

        int rowsAffected = await matchingRequests.ExecuteUpdateAsync(
            e => e.SetProperty(x => x.IsNew, false),
            cancellationToken
        );

        if (rowsAffected == 0)
        {
            throw new DragaliaException(
                ResultCode.ModelUpdateTargetNotFound,
                $"Invalid friend request reset new: expected more than zero rows to be updated"
            );
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task SendRequest(long targetViewerId, CancellationToken cancellationToken)
    {
        DbPlayerFriendRequest? existingRequest =
            await apiContext.PlayerFriendRequests.FirstOrDefaultAsync(
                x =>
                    x.FromPlayerViewerId == targetViewerId
                    && x.ToPlayerViewerId == playerIdentityService.ViewerId,
                cancellationToken
            );

        if (existingRequest is not null)
        {
            // Treat sending two requests to each other the same as accepting the request
            this.LogFriendRequestPair();

            apiContext.PlayerFriendRequests.Remove(existingRequest);
            apiContext.PlayerFriendships.Add(
                CreateFriendship(
                    existingRequest.FromPlayerViewerId,
                    existingRequest.ToPlayerViewerId
                )
            );

            return;
        }

        apiContext.PlayerFriendRequests.Add(
            new DbPlayerFriendRequest()
            {
                FromPlayerViewerId = playerIdentityService.ViewerId,
                ToPlayerViewerId = targetViewerId,
                IsNew = true,
            }
        );
    }

    public async Task<List<UserSupportList>> GetSentRequestList()
    {
        return (
            await apiContext
                .PlayerFriendRequests.Where(x =>
                    x.FromPlayerViewerId == playerIdentityService.ViewerId
                )
                .IgnoreQueryFilters()
                .Select(x => x.ToPlayer!.Helper!)
                .ProjectToHelperProjection()
                .ToListAsync()
        )
            .Select(x => x.MapToUserSupportList())
            .ToList();
    }

    public async Task<List<UserSupportList>> GetReceivedRequestList()
    {
        return (
            await apiContext
                .PlayerFriendRequests.Where(x =>
                    x.ToPlayerViewerId == playerIdentityService.ViewerId
                )
                .IgnoreQueryFilters()
                .Select(x => x.FromPlayer!.Helper!)
                .ProjectToHelperProjection()
                .ToListAsync()
        )
            .Select(x => x.MapToUserSupportList())
            .ToList();
    }

    public async Task CancelRequest(long viewerId, CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction =
            await apiContext.Database.BeginTransactionAsync(cancellationToken);

        int rowsAffected = await apiContext
            .PlayerFriendRequests.Where(x =>
                x.FromPlayerViewerId == playerIdentityService.ViewerId
                && x.ToPlayerViewerId == viewerId
            )
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsAffected != 1)
        {
            throw new DragaliaException(
                ResultCode.FriendApplyError,
                $"Error cancelling friend request to viewer ID {viewerId}: invalid row update count {rowsAffected}"
            );
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<List<UserSupportList>> GetRecommendedFriendsList(
        CancellationToken cancellationToken
    )
    {
        // Choose the most recently active users. This is sort of random assuming you wait
        // long enough between checking the page.
        // It is also a good heuristic as active players are more likely to accept a friend
        // request.
        IQueryable<HelperProjection> eligibleUsers = apiContext
            .Players.IgnoreQueryFilters()
            .Where(x => x.ViewerId != playerIdentityService.ViewerId)
            .Where(x =>
                // Don't suggest people you have already sent a friend request to
                !apiContext.PlayerFriendRequests.Any(y =>
                    y.FromPlayerViewerId == playerIdentityService.ViewerId
                    && y.ToPlayerViewerId == x.ViewerId
                )
            )
            .Where(x =>
                // Don't suggest people who are already your friends
                !apiContext.PlayerFriendships.Any(y =>
                    y.PlayerFriendshipPlayers.Any(z =>
                        z.PlayerViewerId == playerIdentityService.ViewerId
                    ) && y.PlayerFriendshipPlayers.Any(z => z.PlayerViewerId == x.ViewerId)
                )
            )
            .OrderByDescending(x => x.UserData!.LastLoginTime)
            .Where(x => x.Helper != null)
            .Select(x => x.Helper!)
            .ProjectToHelperProjection();

        List<HelperProjection> selectedUsers = await eligibleUsers
            .Take(10)
            .ToListAsync(cancellationToken);

        return selectedUsers.Select(x => x.MapToUserSupportList()).ToList();
    }

    public async Task ReplyToRequest(
        long requestViewerId,
        FriendReplyType replyType,
        CancellationToken cancellationToken
    )
    {
        DbPlayerFriendRequest? requestEntity =
            await apiContext.PlayerFriendRequests.FirstOrDefaultAsync(
                x =>
                    x.FromPlayerViewerId == requestViewerId
                    && x.ToPlayerViewerId == playerIdentityService.ViewerId,
                cancellationToken
            );

        if (requestEntity is null)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Cannot reply to friend request from viewer ID {requestViewerId} - does not exist"
            );
        }

        apiContext.PlayerFriendRequests.Remove(requestEntity);

        if (replyType == FriendReplyType.Accept)
        {
            apiContext.PlayerFriendships.Add(
                CreateFriendship(requestEntity.FromPlayerViewerId, requestEntity.ToPlayerViewerId)
            );
        }
    }

    public async Task DeleteFriend(long viewerId, CancellationToken cancellationToken)
    {
        DbPlayerFriendship? friendship = await apiContext
            .PlayerFriendships.Where(x =>
                x.PlayerFriendshipPlayers.Any(y =>
                    y.PlayerViewerId == playerIdentityService.ViewerId
                ) && x.PlayerFriendshipPlayers.Any(y => y.PlayerViewerId == viewerId)
            )
            .Include(x => x.PlayerFriendshipPlayers)
            .FirstOrDefaultAsync(cancellationToken);

        if (friendship is null)
        {
            throw new DragaliaException(
                ResultCode.FriendDeleteError,
                "Could not find friendship to delete"
            );
        }

        apiContext.PlayerFriendshipPlayers.RemoveRange(friendship.PlayerFriendshipPlayers);
        apiContext.PlayerFriendships.Remove(friendship);
    }

    /// <summary>
    /// Gets a subset of the provided list of IDs representing which are friends with the current player.
    /// </summary>
    /// <param name="filterIds">The IDs to filter by.</param>
    /// <returns>A subset of <param name="filterIds"/> containing friends.</returns>
    public async Task<List<long>> CheckFriendStatus(IEnumerable<long> filterIds)
    {
        return await this.GetFriendsQuery()
            .Where(x => filterIds.Contains(x.ViewerId))
            .Select(x => x.ViewerId)
            .ToListAsync();
    }

    private IQueryable<DbPlayer> GetFriendsQuery()
    {
        IQueryable<DbPlayer> currentPlayer = apiContext.Players.Where(x =>
            x.ViewerId == playerIdentityService.ViewerId
        );

        return currentPlayer
            .SelectMany(x => x.Friendships)
            .SelectMany(x => x.Players)
            .Where(x => x.ViewerId != playerIdentityService.ViewerId)
            .IgnoreQueryFilters();
    }

    private static DbPlayerFriendship CreateFriendship(long player1Id, long player2Id) =>
        new DbPlayerFriendship()
        {
            PlayerFriendshipPlayers =
            [
                new() { PlayerViewerId = player1Id, IsNew = true },
                new() { PlayerViewerId = player2Id, IsNew = true },
            ],
        };

    [LoggerMessage(LogLevel.Information, "Detected friend request pair - creating friendship")]
    private partial void LogFriendRequestPair();
}
