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
        IDbContextTransaction transaction = await apiContext.Database.BeginTransactionAsync(
            cancellationToken
        );

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

    public async Task<ResultCode> SendRequest(long targetViewerId)
    {
        apiContext.PlayerFriendRequests.Add(
            new DbPlayerFriendRequest()
            {
                FromPlayerViewerId = playerIdentityService.ViewerId,
                ToPlayerViewerId = targetViewerId,
                IsNew = true,
            }
        );

        try
        {
            await apiContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            this.LogFriendRequestExists(targetViewerId);
            return ResultCode.FriendApplyExists;
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation())
        {
            // Should never happen on a legitimate client
            this.LogFriendRequestTargetDoesNotExist(targetViewerId);
            return ResultCode.FriendApplyError;
        }

        return ResultCode.Success;
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

    [LoggerMessage(
        LogLevel.Information,
        "Friend request to {ViewerId} failed: request already exists"
    )]
    private partial void LogFriendRequestExists(long viewerId);

    [LoggerMessage(LogLevel.Error, "Friend request to {ViewerId} failed: invalid viewer ID")]
    private partial void LogFriendRequestTargetDoesNotExist(long viewerId);
}
