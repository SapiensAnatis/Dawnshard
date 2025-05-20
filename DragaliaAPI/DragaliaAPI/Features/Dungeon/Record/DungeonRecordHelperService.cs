using DragaliaAPI.Database;
using DragaliaAPI.Features.CoOp;
using DragaliaAPI.Features.Friends;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon.Record;

internal sealed class DungeonRecordHelperService(
    IPlayerIdentityService playerIdentityService,
    ApiContext apiContext,
    IHelperService helperService,
    IMatchingService matchingService,
    FriendService friendService,
    ILogger<DungeonRecordHelperService> logger
) : IDungeonRecordHelperService
{
    public async Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList,
        int RewardMana
    )> ProcessHelperDataSolo(ulong? supportViewerId)
    {
        List<UserSupportList> helperList = new();
        List<AtgenHelperDetailList> helperDetailList = new();

        if (supportViewerId is null)
        {
            return (helperList, helperDetailList, 0);
        }

        AtgenSupportUserDataDetail? supportList = await helperService.GetHelperDetail(
            (long)supportViewerId.Value
        );

        int rewardMana = 25;

        if (supportList is not null)
        {
            await helperService.UseHelper((long)supportViewerId.Value);

            helperList.Add(supportList.UserSupportData);

            helperDetailList.Add(
                new AtgenHelperDetailList()
                {
                    ViewerId = supportList.UserSupportData.ViewerId,
                    IsFriend = supportList.IsFriend,
                    ApplySendStatus = supportList.ApplySendStatus,
                    GetManaPoint = supportList.IsFriend
                        ? HelperConstants.HelperFriendRewardMana
                        : HelperConstants.HelperRewardMana,
                }
            );

            if (supportList.IsFriend)
            {
                rewardMana = 50;
            }
        }

        return (helperList, helperDetailList, rewardMana);
    }

    public async Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList
    )> ProcessHelperDataMulti()
    {
        List<long> connectingViewerIdList = (await matchingService.GetTeammates())
            .Select(x => x.ViewerId)
            .ToList();

        return await this.ProcessHelperDataMulti(connectingViewerIdList);
    }

    // TODO: test with empty weapon / dragon / print slots / etc
    public async Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList
    )> ProcessHelperDataMulti(IList<long> connectingViewerIdList)
    {
        IEnumerable<UserSupportList> teammateSupportLists = await this.GetTeammateSupportList(
            connectingViewerIdList
        );

        List<(long ViewerId, bool IsFriend, bool HasFriendRequest)> friendCheckList =
            await friendService.CheckFriendStatus(connectingViewerIdList);

        logger.LogDebug("Retrieved teammate support list {@supportList}", teammateSupportLists);

        IEnumerable<AtgenHelperDetailList> teammateDetailLists = friendCheckList.Select(
            x => new AtgenHelperDetailList()
            {
                IsFriend = x.IsFriend,
                ViewerId = (ulong)x.ViewerId,
                GetManaPoint = x.IsFriend
                    ? HelperConstants.HelperFriendRewardMana
                    : HelperConstants.HelperRewardMana,
                ApplySendStatus = x.HasFriendRequest ? 1 : 0,
            }
        );

        return (teammateSupportLists, teammateDetailLists);
    }

    private async Task<List<UserSupportList>> GetTeammateSupportList(
        IList<long> connectingViewerIdList
    )
    {
        List<UserSupportList> helperList = [];

        var userDetails = await apiContext
            .PlayerUserData.IgnoreQueryFilters()
            .Where(x => connectingViewerIdList.Contains(x.ViewerId))
            .ToDictionaryAsync(x => x.ViewerId, x => new { x.ViewerId, x.MainPartyNo });

        foreach (long viewerId in connectingViewerIdList)
        {
            if (!userDetails.TryGetValue(viewerId, out var userData))
            {
                logger.LogWarning("No user details returned for viewer ID {ViewerId}", viewerId);
                continue;
            }

            using IDisposable impersonationCtx = playerIdentityService.StartUserImpersonation(
                userData.ViewerId
            );

            try
            {
                UserSupportList leadUnit = await helperService.GetLeadUnit(userData.MainPartyNo);
                helperList.Add(leadUnit);
            }
            catch (Exception e)
            {
                logger.LogWarning(
                    e,
                    "Failed to populate multiplayer support info for viewer ID {ViewerId}",
                    viewerId
                );
            }
        }

        return helperList;
    }
}
