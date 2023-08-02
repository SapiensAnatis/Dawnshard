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


    public WallController(
        IBonusService bonusService,
        IUpdateDataService updateDataService,
        IRewardService rewardService,
        IClearPartyService clearPartyService,
        IDungeonService dungeonService
    )
    {
        this.bonusService = bonusService;
        this.updateDataService = updateDataService;
        this.rewardService = rewardService;
        this.clearPartyService = clearPartyService;
        this.dungeonService = dungeonService;
    }

    
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
                {
                    quest_id = session.QuestData.Id,
                    wall_id = 0,
                    wall_level = 0,
                    is_host = true,
                }
            }
        );
    }
    

    /*
    [HttpPost("get_monthly_reward")]
    public async Task<DragaliaResult> GetMonthlyReward()
    {
        return Ok(data);
    }
    */

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

    /*
    [HttpPost("receive_monthly_reward")]
    public async Task<DragaliaResult> ReceiveMonthlyReward()
    {
        return Ok(data);
    }

    [HttpPost("set_wall_clear_party")]
    public async Task<DragaliaResult> SetWallClearParty()
    {
        return Ok(data);
    }
    */
}
