using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using PhotonPlayer = DragaliaAPI.Photon.Shared.Models.Player;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordHelperService(
    IPlayerIdentityService playerIdentityService,
    IUserDataRepository userDataRepository,
    IHelperService helperService,
    IMatchingService matchingService,
    ILogger<DungeonRecordHelperService> logger
) : IDungeonRecordHelperService
{
    public async Task<IngameResultData> ProcessHelperDataSolo(
        IngameResultData resultData,
        ulong? supportViewerId
    )
    {
        if (supportViewerId is null)
            return resultData;

        UserSupportList? supportList = await helperService.GetHelper(supportViewerId.Value);

        if (supportList is null)
            return resultData;

        resultData.helper_list = new List<UserSupportList>() { supportList };

        // TODO: Replace with friend system once implemented
        resultData.helper_detail_list = new List<AtgenHelperDetailList>()
        {
            new()
            {
                viewer_id = supportList.viewer_id,
                is_friend = true,
                apply_send_status = 1,
                get_mana_point = 50
            }
        };

        return resultData;
    }

    // TODO: test with empty weapon / dragon / print slots / etc
    public async Task<IngameResultData> ProcessHelperDataMulti(IngameResultData resultData)
    {
        IEnumerable<PhotonPlayer> teammates = await matchingService.GetTeammates();

        IEnumerable<UserSupportList> teammateSupportLists = await this.GetTeammateSupportList(
            teammates
        );

        // TODO: Replace with friend system once implemented
        IEnumerable<AtgenHelperDetailList> teammateDetailLists = teammates.Select(
            x =>
                new AtgenHelperDetailList()
                {
                    is_friend = true,
                    viewer_id = (ulong)x.ViewerId,
                    get_mana_point = 50,
                    apply_send_status = 0,
                }
        );

        resultData.helper_list = teammateSupportLists;
        resultData.helper_detail_list = teammateDetailLists;

        return resultData;
    }

    private async Task<IEnumerable<UserSupportList>> GetTeammateSupportList(
        IEnumerable<PhotonPlayer> teammates
    )
    {
        List<UserSupportList> helperList = new();

        Dictionary<long, DbPlayerUserData> userDetails = await userDataRepository
            .GetMultipleViewerData(teammates.Select(x => x.ViewerId))
            .ToDictionaryAsync(x => x.ViewerId, x => x);

        foreach (PhotonPlayer player in teammates)
        {
            if (!userDetails.TryGetValue(player.ViewerId, out DbPlayerUserData? userData))
            {
                logger.LogDebug("No user details returned for player {@player}", player);
                continue;
            }

            using IDisposable impersonationCtx = playerIdentityService.StartUserImpersonation(
                userData.DeviceAccountId,
                userData.ViewerId
            );

            try
            {
                UserSupportList leadUnit = await helperService.GetLeadUnit(
                    player.PartyNoList.First()
                );

                helperList.Add(leadUnit);
            }
            catch (Exception e)
            {
                logger.LogDebug(
                    e,
                    "Failed to populate multiplayer support info for player {@player}",
                    player
                );
                continue;
            }
        }

        return helperList;
    }
}
