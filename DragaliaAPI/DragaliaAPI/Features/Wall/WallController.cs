using DragaliaAPI.Controllers;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Wall;

[Route("wall")]
public class WallController : DragaliaControllerBase
{
    private readonly IUpdateDataService updateDataService;
    private readonly IRewardService rewardService;
    private readonly IClearPartyService clearPartyService;
    private readonly IDungeonService dungeonService;
    private readonly IWallService wallService;

    public WallController(
        IUpdateDataService updateDataService,
        IRewardService rewardService,
        IClearPartyService clearPartyService,
        IDungeonService dungeonService,
        IWallService wallService
    )
    {
        this.updateDataService = updateDataService;
        this.rewardService = rewardService;
        this.clearPartyService = clearPartyService;
        this.dungeonService = dungeonService;
        this.wallService = wallService;
    }

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

    // When is this called?
    [HttpPost("get_monthly_reward")]
    public async Task<DragaliaResult> GetMonthlyReward()
    {
        int totalLevel = await wallService.GetTotalWallLevel();

        IEnumerable<AtgenUserWallRewardList> userWallRewardList = wallService.GetUserWallRewardList(
            totalLevel,
            RewardStatus.Received
        );

        WallGetMonthlyRewardResponse data = new() { UserWallRewardList = userWallRewardList };

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
            await this.clearPartyService.GetQuestClearParty(request.WallId, false);

        await this.updateDataService.SaveChangesAsync(cancellationToken); // Updated lost entities

        WallGetWallClearPartyResponse data =
            new() { WallClearPartySettingList = clearParty, LostUnitList = lostUnitList };
        return Ok(data);
    }

    // Called upon entering the MG menu when the user is available to receive
    // monthly MG rewards (i assume)
    [HttpPost("receive_monthly_reward")]
    public async Task<DragaliaResult> ReceiveMonthlyReward(
        WallReceiveMonthlyRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        int totalLevel = await wallService.GetTotalWallLevel();

        IEnumerable<AtgenBuildEventRewardEntityList> rewardEntityList =
            wallService.GetMonthlyRewardEntityList(totalLevel);

        IEnumerable<AtgenUserWallRewardList> userWallRewardList = wallService.GetUserWallRewardList(
            totalLevel,
            RewardStatus.Received
        );

        // Grant Rewards
        await wallService.GrantMonthlyRewardEntityList(rewardEntityList);

        EntityResult entityResult = this.rewardService.GetEntityResult();

        AtgenMonthlyWallReceiveList monthlyWallReceiveList =
            new()
            {
                QuestGroupId = WallService.WallQuestGroupId,
                IsReceiveReward = RewardStatus.Received
            };

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        WallReceiveMonthlyRewardResponse data =
            new()
            {
                UpdateDataList = updateDataList,
                EntityResult = entityResult,
                WallMonthlyRewardList = rewardEntityList,
                UserWallRewardList = userWallRewardList,
                MonthlyWallReceiveList = new[] { monthlyWallReceiveList }
            };

        return Ok(data);
    }

    // Called upon clearing a MG quest and then clicking on the Next button
    // what does this actually do?
    [HttpPost("set_wall_clear_party")]
    public async Task<DragaliaResult> SetWallClearParty(
        WallSetWallClearPartyRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.clearPartyService.SetQuestClearParty(
            request.WallId,
            false,
            request.RequestPartySettingList
        );

        await this.updateDataService.SaveChangesAsync(cancellationToken);

        WallSetWallClearPartyResponse data = new() { Result = 1 };

        return Ok(data);
    }
}
