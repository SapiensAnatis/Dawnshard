using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DragaliaAPI.Features.Friends;

internal sealed class FriendService(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
)
{
    public async Task<int> GetFriendCount()
    {
        return await this.GetFriendsQuery().CountAsync();
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

    public async Task<List<long>> GetNewFriendViewerIdList()
    {
        return await apiContext
            .PlayerFriendshipPlayers.IgnoreQueryFilters()
            .Where(x => x.PlayerViewerId == playerIdentityService.ViewerId && x.IsNew)
            .Select(x => x.Friendship)
            .SelectMany(x => x!.Players)
            .Select(x => x.ViewerId)
            .Where(x => x != playerIdentityService.ViewerId)
            .ToListAsync();
    }

    public async Task ResetNew(IEnumerable<long> targetIdList)
    {
        IList<long> targetIdListEnumerated = targetIdList as IList<long> ?? targetIdList.ToList();

        IDbContextTransaction transaction = await apiContext.Database.BeginTransactionAsync();

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

        int rowsAffected = await friendshipsToUpdate.ExecuteUpdateAsync(e =>
            e.SetProperty(x => x.IsNew, false)
        );

        if (rowsAffected != targetIdListEnumerated.Count)
        {
            throw new DragaliaException(
                ResultCode.ModelUpdateTargetNotFound,
                $"Invalid friendship reset new: expected {targetIdListEnumerated.Count} rows to be updated, got {rowsAffected}"
            );
        }

        await transaction.CommitAsync();
    }

    private IQueryable<DbPlayer> GetFriendsQuery()
    {
        IQueryable<DbPlayer> currentPlayer = apiContext.Players.Where(x =>
            x.ViewerId == playerIdentityService.ViewerId
        );

        return currentPlayer
            .SelectMany(x => x.Friendships)
            .Where(x => x.IsAccepted)
            .SelectMany(x => x.Players)
            .Where(x => x.ViewerId != playerIdentityService.ViewerId)
            .IgnoreQueryFilters();
    }
}
