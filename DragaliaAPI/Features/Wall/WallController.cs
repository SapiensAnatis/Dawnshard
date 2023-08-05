using System.Runtime.CompilerServices;
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

namespace DragaliaAPI.Features.Wall;

[Route("wall")]
public class WallController : DragaliaControllerBase
{
    private readonly IBonusService bonusService;
    private readonly IUpdateDataService updateDataService;
    private readonly IRewardService rewardService;
    private readonly IClearPartyService clearPartyService;
    private readonly IDungeonService dungeonService;
    private readonly IWallService wallService;


    public WallController(
        IBonusService bonusService,
        IUpdateDataService updateDataService,
        IRewardService rewardService,
        IClearPartyService clearPartyService,
        IDungeonService dungeonService,
        IWallService wallService
    )
    {
        this.bonusService = bonusService;
        this.updateDataService = updateDataService;
        this.rewardService = rewardService;
        this.clearPartyService = clearPartyService;
        this.dungeonService = dungeonService;
        this.wallService = wallService;
    }


    // Called when failing a MG quest
    // should return the ID and level of the failed quest in the response...
    // but not sure how to get that from the dungeon key in the request currently
    [HttpPost("fail")]
    public async Task<DragaliaResult> Fail(WallFailRequest request)
    {
        DungeonSession session = await dungeonService.FinishDungeon(request.dungeon_key);

        return Ok(
            new DungeonFailData()
            {
                result = 1,
                fail_helper_list = new List<UserSupportList>(),
                fail_helper_detail_list = new List<AtgenHelperDetailList>(),
                fail_quest_detail = new()
                { //stub
                    quest_id = session.QuestData.Id,
                    wall_id = 0,
                    wall_level = 0,
                    is_host = true,
                }
            }
        );
    }



    [HttpPost("get_monthly_reward")]
    public async Task<DragaliaResult> GetMonthlyReward(WallGetMonthlyRewardRequest request)
    {
      
        AtgenUserWallRewardList wallRewardList =
            new()
            {
                quest_group_id = request.quest_group_id,
                sum_wall_level = await wallService.GetTotalWallLevel(),
                last_reward_date = DateTimeOffset.UtcNow,
                reward_status = RewardStatus.Received
            };

        WallGetMonthlyRewardData data =
            new()
            {
                user_wall_reward_list = new[] { wallRewardList } 
            };

        return Ok(data);
    }

    // Called when entering the team edit screen
    [HttpPost("get_wall_clear_party")]
    public async Task<DragaliaResult> GetWallClearParty(WallGetWallClearPartyRequest request)
    {
        (IEnumerable<PartySettingList> clearParty, IEnumerable<AtgenLostUnitList> lostUnitList) =
            await this.clearPartyService.GetQuestClearParty(0, false);

        await this.updateDataService.SaveChangesAsync(); // Updated lost entities

        WallGetWallClearPartyData data =
            new()
            {
                wall_clear_party_setting_list = clearParty,
                lost_unit_list = lostUnitList
            };
        return Ok(data);
    }


    [HttpPost("receive_monthly_reward")]
    public async Task<DragaliaResult> ReceiveMonthlyReward(WallReceiveMonthlyRewardRequest request)
    {

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        //stub
        WallReceiveMonthlyRewardData data =
            new()
            {
                update_data_list = updateDataList,
                entity_result = null,
                wall_monthly_reward_list = null,
                user_wall_reward_list = null,
                monthly_wall_receive_list = null
            };

        return Ok(data);
    }


    // Called upon clearing a MG quest and then clicking on the Next button
    [HttpPost("set_wall_clear_party")] 
    public async Task<DragaliaResult> SetWallClearParty(WallSetWallClearPartyRequest request)
    {

        WallSetWallClearPartyData data =
            new()
            {
                result = 1
            };

        return Ok(data);
    }

}
