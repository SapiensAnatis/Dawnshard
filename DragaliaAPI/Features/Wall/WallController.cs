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
        DungeonSession session = await dungeonService.FinishDungeon(request.dungeon_key);

        return Ok(
            new WallFailData()
            {
                result = 1,
                fail_helper_list = new List<UserSupportList>(),
                fail_helper_detail_list = new List<AtgenHelperDetailList>(),
                fail_quest_detail = new()
                {
                    wall_id = session.WallId,
                    wall_level = session.WallLevel,
                    is_host = true,
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

        WallGetMonthlyRewardData data = new() { user_wall_reward_list = userWallRewardList };

        return Ok(data);
    }

    // Called when entering the team edit screen
    [HttpPost("get_wall_clear_party")]
    public async Task<DragaliaResult> GetWallClearParty(WallGetWallClearPartyRequest request)
    {
        (IEnumerable<PartySettingList> clearParty, IEnumerable<AtgenLostUnitList> lostUnitList) =
            await this.clearPartyService.GetQuestClearParty(request.wall_id, false);

        await this.updateDataService.SaveChangesAsync(); // Updated lost entities

        WallGetWallClearPartyData data =
            new() { wall_clear_party_setting_list = clearParty, lost_unit_list = lostUnitList };
        return Ok(data);
    }

    // Called upon entering the MG menu when the user is available to receive
    // monthly MG rewards (i assume)
    [HttpPost("receive_monthly_reward")]
    public async Task<DragaliaResult> ReceiveMonthlyReward(WallReceiveMonthlyRewardRequest request)
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
                quest_group_id = WallService.WallQuestGroupId,
                is_receive_reward = RewardStatus.Received
            };

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        WallReceiveMonthlyRewardData data =
            new()
            {
                update_data_list = updateDataList,
                entity_result = entityResult,
                wall_monthly_reward_list = rewardEntityList,
                user_wall_reward_list = userWallRewardList,
                monthly_wall_receive_list = new[] { monthlyWallReceiveList }
            };

        return Ok(data);
    }

    // Called upon clearing a MG quest and then clicking on the Next button
    // what does this actually do?
    [HttpPost("set_wall_clear_party")]
    public async Task<DragaliaResult> SetWallClearParty(WallSetWallClearPartyRequest request)
    {
        await this.clearPartyService.SetQuestClearParty(
            request.wall_id,
            false,
            request.request_party_setting_list
        );

        await this.updateDataService.SaveChangesAsync();

        WallSetWallClearPartyData data = new() { result = 1 };

        return Ok(data);
    }
}
