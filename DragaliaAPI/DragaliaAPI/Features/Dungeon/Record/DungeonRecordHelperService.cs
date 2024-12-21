using DragaliaAPI.Database;
using DragaliaAPI.Features.CoOp;
using DragaliaAPI.Features.Friends;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordHelperService(
    IPlayerIdentityService playerIdentityService,
    ApiContext apiContext,
    IHelperService helperService,
    IMatchingService matchingService,
    ILogger<DungeonRecordHelperService> logger
) : IDungeonRecordHelperService
{
    public async Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList
    )> ProcessHelperDataSolo(ulong? supportViewerId)
    {
        List<UserSupportList> helperList = new();
        List<AtgenHelperDetailList> helperDetailList = new();

        if (supportViewerId is null)
            return (helperList, helperDetailList);

        UserSupportList? supportList = await helperService.GetHelper(supportViewerId.Value);

        if (supportList is not null)
        {
            helperList.Add(supportList);

            // TODO: Replace with friends system once fully added
            helperDetailList.Add(
                new AtgenHelperDetailList()
                {
                    ViewerId = supportList.ViewerId,
                    IsFriend = true,
                    ApplySendStatus = 1,
                    GetManaPoint = 50,
                }
            );
        }

        return (helperList, helperDetailList);
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

        logger.LogDebug("Retrieved teammate support list {@supportList}", teammateSupportLists);

        // TODO: Replace with friend system once implemented
        IEnumerable<AtgenHelperDetailList> teammateDetailLists = connectingViewerIdList.Select(
            x => new AtgenHelperDetailList()
            {
                IsFriend = true,
                ViewerId = (ulong)x,
                GetManaPoint = 50,
                ApplySendStatus = 0,
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
