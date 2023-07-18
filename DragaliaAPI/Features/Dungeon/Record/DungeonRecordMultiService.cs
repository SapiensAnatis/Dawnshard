using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordMultiService(
    IPlayerIdentityService playerIdentityService,
    IUserDataRepository userDataRepository,
    ILogger<DungeonRecordMultiService> logger,
    IHelperService helperService
) : IDungeonRecordMultiService
{
    public async Task<IEnumerable<UserSupportList>> GetTeammateSupportList(
        IEnumerable<Player> teammates
    )
    {
        List<UserSupportList> helperList = new();

        Dictionary<long, DbPlayerUserData> userDetails = await userDataRepository
            .GetMultipleViewerData(teammates.Select(x => x.ViewerId))
            .ToDictionaryAsync(x => x.ViewerId, x => x);

        foreach (Player player in teammates)
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
