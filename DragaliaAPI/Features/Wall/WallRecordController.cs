using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Wall;

[Route("wall_record")]
public class WallRecordController : DragaliaControllerBase
{
    private readonly IUpdateDataService updateDataService;
    private readonly IWallRepository wallRepository;
    private readonly IWallService wallService;
    private readonly IRewardService rewardService;
    private readonly IDungeonService dungeonService;
    private readonly IPresentService presentService;
    private readonly ILogger<WallRecordController> logger;

    public WallRecordController(
        IUpdateDataService updateDataService,
        IWallRepository wallRepository,
        IWallService wallService,
        IRewardService rewardService,
        IDungeonService dungeonService,
        IPresentService presentService,
        ILogger<WallRecordController> logger
    )
    {
        this.updateDataService = updateDataService;
        this.wallRepository = wallRepository;
        this.wallService = wallService;
        this.rewardService = rewardService;
        this.dungeonService = dungeonService;
        this.presentService = presentService;
        this.logger = logger;
    }

    // Called upon completing a MG quest
    [HttpPost("record")]
    public async Task<DragaliaResult> Record(WallRecordRecordRequest request)
    {
        DungeonSession dungeonSession = await dungeonService.FinishDungeon(request.dungeon_key);

        DbPlayerQuestWall questWall = await wallRepository.GetQuestWall(request.wall_id);

        int previousLevel = questWall.WallLevel;

        // Don't grant first clear wyrmite if you are re-clearing the last level
        bool toGrantFirstClearWyrmite = previousLevel != WallService.MaximumQuestWallLevel;

        logger.LogInformation("[wall_record/record] Cleared wall quest with 'wall_id' {@wall_id} and 'wall_level' {@wall_level}",
            request.wall_id, questWall.WallLevel);

        // Level up completed wall quest
        await wallService.LevelupQuestWall(request.wall_id);

        // Response data
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
                reward_entity_list = new[] { GoldCrystals.ToBuildEventRewardEntityList() },
                take_coin = 500,
                take_mana = 120
            };

        AtgenPlayWallDetail playWallDetail =
            new()
            {
                wall_id = request.wall_id,
                before_wall_level = previousLevel,
                after_wall_level = previousLevel + 1
            };

        // Grant Rewards
        await rewardService.GrantReward(GoldCrystals);
        
        await rewardService.GrantReward(
            new Entity(EntityTypes.Rupies, 1, 500)
        );
        
        await rewardService.GrantReward(
            new Entity(EntityTypes.Mana, 0, 120)
        );
        
        if (toGrantFirstClearWyrmite)
        {
            presentService.AddPresent(
                new Present.Present(
                    PresentMessage.FirstClear,
                    Wyrmites.Type,
                    Wyrmites.Id,
                    Wyrmites.Quantity
                )
            );
        }

        IEnumerable<AtgenBuildEventRewardEntityList> wallClearRewardList =
            toGrantFirstClearWyrmite ?
            new[] { Wyrmites.ToBuildEventRewardEntityList() } :
            Enumerable.Empty<AtgenBuildEventRewardEntityList>();

        EntityResult entityResult = rewardService.GetEntityResult();

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        WallRecordRecordData data =
            new()
            {
                update_data_list = updateDataList,
                entity_result = entityResult,
                play_wall_detail = playWallDetail,
                wall_clear_reward_list = wallClearRewardList,
                wall_drop_reward = wallDropReward,
                wall_unit_info = wallUnitInfo
            };
        return Ok(data);

    }

    private readonly Entity GoldCrystals = new(EntityTypes.Material, (int)Materials.GoldCrystal, 3);

    private readonly Entity Wyrmites = new(EntityTypes.Wyrmite, 0, 10);

}
