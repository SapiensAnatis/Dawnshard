using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DragaliaAPI.Features.Wall;

[Route("wall_record")]
public class WallRecordController : DragaliaControllerBase
{
    private readonly IUpdateDataService updateDataService;
    private readonly IRewardService rewardService;
    private readonly IDungeonService dungeonService;
    private readonly ILogger<WallStartController> logger;

    public WallRecordController(
        IUpdateDataService updateDataService,
        IRewardService rewardService,
        IDungeonService dungeonService,
        ILogger<WallStartController> logger
    )
    {
        this.updateDataService = updateDataService;
        this.rewardService = rewardService;
        this.dungeonService = dungeonService;
        this.logger = logger;
    }


    [HttpPost("record")]
    public async Task<DragaliaResult> Record(WallRecordRecordRequest request)
    {
        DungeonSession dungeonSession = dungeonService.FinishDungeon(request.dungeon_key).Result;

        int current_level = 0;

        AtgenWallUnitInfo wallUnitInfo =
            new()
            {
                quest_party_setting_list = dungeonSession.Party,
                helper_list = new List<UserSupportList>(),
                helper_detail_list = new List<AtgenHelperDetailList>()
            };

        AtgenWallDropReward wallDropReward =
            new()
            {
                reward_entity_list = GOLD_CRYSTALS,
                take_coin = 500,
                take_mana = 120
            };

        AtgenPlayWallDetail playWallDetail =
            new()
            {
                wall_id = request.wall_id,
                before_wall_level = 0,
                after_wall_level = 1
            };

        EntityResult entityResult = rewardService.GetEntityResult();

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        WallRecordRecordData data =
            new()
            {
                update_data_list = updateDataList,
                entity_result = entityResult,
                play_wall_detail = playWallDetail,
                wall_clear_reward_list = WYRMITES,
                wall_drop_reward = wallDropReward,
                wall_unit_info = wallUnitInfo
            };
        return Ok(data);

    }

    private IEnumerable<AtgenBuildEventRewardEntityList> GOLD_CRYSTALS =
        new List<AtgenBuildEventRewardEntityList>() {
            new AtgenBuildEventRewardEntityList ()
            {
                entity_type = EntityTypes.Material,
                entity_id = (int)Materials.GoldCrystal,
                entity_quantity = 3
            }
        };

    private IEnumerable<AtgenBuildEventRewardEntityList> WYRMITES =
        new List<AtgenBuildEventRewardEntityList>() {
            new AtgenBuildEventRewardEntityList ()
            {
                entity_type = EntityTypes.Wyrmite,
                entity_id = 0,
                entity_quantity = 10
            }
        };
}
