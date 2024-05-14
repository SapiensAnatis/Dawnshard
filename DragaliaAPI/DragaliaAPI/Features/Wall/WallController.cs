using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DragaliaAPI.Features.Wall;

[Route("wall")]
public partial class WallController(
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IClearPartyService clearPartyService,
    IDungeonService dungeonService,
    IWallService wallService,
    ILogger<WallController> logger
) : DragaliaControllerBase
{
    [HttpPost("fail")]
    public async Task<DragaliaResult> Fail(WallFailRequest request)
    {
        DungeonSession session = await dungeonService.FinishDungeon(request.DungeonKey);

        return Ok(
            new WallFailResponse()
            {
                Result = 1,
                FailHelperList = new List<UserSupportList>(),
                FailHelperDetailList = new List<AtgenHelperDetailList>(),
                FailQuestDetail = new()
                {
                    WallId = session.WallId,
                    WallLevel = session.WallLevel,
                    IsHost = true,
                }
            }
        );
    }

    [HttpPost("get_monthly_reward")]
    public async Task<DragaliaResult> GetMonthlyReward()
    {
        if (!await wallService.CheckWallInitialized())
        {
            Log.InvalidCheckAttempt(logger);

            return this.Code(
                ResultCode.CommonInvalidArgument,
                "Invalid attempt to claim wall monthly reward: not initialized"
            );
        }

        AtgenUserWallRewardList userWallRewardList = await wallService.GetUserWallRewardList();

        WallGetMonthlyRewardResponse data = new() { UserWallRewardList = [userWallRewardList] };

        return Ok(data);
    }

    // Called when entering the team edit screen
    [HttpPost("get_wall_clear_party")]
    public async Task<DragaliaResult> GetWallClearParty(
        WallGetWallClearPartyRequest request,
        CancellationToken cancellationToken
    )
    {
        (IEnumerable<PartySettingList> clearParty, IEnumerable<AtgenLostUnitList> lostUnitList) =
            await clearPartyService.GetQuestClearParty(request.WallId, false);

        await updateDataService.SaveChangesAsync(cancellationToken); // Updated lost entities

        WallGetWallClearPartyResponse data =
            new() { WallClearPartySettingList = clearParty, LostUnitList = lostUnitList };
        return Ok(data);
    }

    [HttpPost("receive_monthly_reward")]
    public async Task<DragaliaResult> ReceiveMonthlyReward(CancellationToken cancellationToken)
    {
        // Called when sending `monthly_wall_reward_list` from /login/index

        if (!await wallService.CheckWallInitialized())
        {
            Log.InvalidClaimAttempt(logger);

            return this.Code(
                ResultCode.CommonInvalidArgument,
                "Invalid attempt to claim wall monthly reward: not initialized"
            );
        }

        // Retrieve to track and update in GrantMonthlyRewardEntityList later
        DbWallRewardDate lastRewardDate = await wallService.GetLastRewardDate();

        if (!wallService.CheckCanClaimReward(lastRewardDate.LastClaimDate))
        {
            Log.InvalidClaimAttempt(logger);

            return this.Code(
                ResultCode.CommonInvalidArgument,
                "Invalid attempt to claim wall monthly reward: not eligible"
            );
        }

        int totalLevel = await wallService.GetTotalWallLevel();

        List<AtgenBuildEventRewardEntityList> rewardEntityList =
            wallService.GetMonthlyRewardEntityList(totalLevel);

        AtgenUserWallRewardList userWallRewardList = await wallService.GetUserWallRewardList();

        // Grant Rewards
        await wallService.GrantMonthlyRewardEntityList(rewardEntityList);

        EntityResult entityResult = rewardService.GetEntityResult();

        AtgenMonthlyWallReceiveList monthlyWallReceiveList =
            new()
            {
                QuestGroupId = WallService.WallQuestGroupId,
                IsReceiveReward = RewardStatus.Received
            };

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        WallReceiveMonthlyRewardResponse data =
            new()
            {
                UpdateDataList = updateDataList,
                EntityResult = entityResult,
                WallMonthlyRewardList = rewardEntityList,
                UserWallRewardList = [userWallRewardList],
                MonthlyWallReceiveList = [monthlyWallReceiveList]
            };

        return Ok(data);
    }

    [HttpPost("set_wall_clear_party")]
    public async Task<DragaliaResult> SetWallClearParty(
        WallSetWallClearPartyRequest request,
        CancellationToken cancellationToken
    )
    {
        // Called upon clearing an MG quest and then clicking on the Next button

        await clearPartyService.SetQuestClearParty(
            request.WallId,
            false,
            request.RequestPartySettingList
        );

        await updateDataService.SaveChangesAsync(cancellationToken);

        WallSetWallClearPartyResponse data = new() { Result = 1 };

        return Ok(data);
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Error, "Invalid attempt to claim wall reward")]
        public static partial void InvalidClaimAttempt(ILogger logger);

        [LoggerMessage(LogLevel.Error, "Invalid attempt to check wall reward")]
        public static partial void InvalidCheckAttempt(ILogger logger);
    }
}
