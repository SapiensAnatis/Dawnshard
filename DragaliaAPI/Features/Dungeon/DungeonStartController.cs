using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon;

[ApiController]
[Route("dungeon_start")]
public class DungeonStartController : DragaliaControllerBase
{
    private readonly IDungeonStartService dungeonStartService;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<DungeonStartController> logger;

    public DungeonStartController(
        IDungeonStartService dungeonStartService,
        IUpdateDataService updateDataService,
        ILogger<DungeonStartController> logger
    )
    {
        this.dungeonStartService = dungeonStartService;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost("start")]
    public async Task<DragaliaResult> Start(DungeonStartStartRequest request)
    {
        logger.LogDebug("Starting dungeon for quest id {questId}", request.quest_id);

        IngameQuestData ingameQuestData = await this.dungeonStartService.InitiateQuest(
            request.quest_id
        );

        UpdateDataList updateData = await this.updateDataService.SaveChangesAsync();

        IngameData ingameData = await this.dungeonStartService.GetIngameData(
            request.quest_id,
            request.party_no_list,
            request.support_viewer_id
        );

        this.logger.LogDebug("Issued dungeon key {dungeonKey}", ingameData.dungeon_key);

        DungeonStartStartData response =
            new()
            {
                ingame_data = ingameData,
                ingame_quest_data = ingameQuestData,
                odds_info = StubData.OddsInfo,
                update_data_list = updateData
            };

        return this.Ok(response);
    }

    [HttpPost("start_multi")]
    public async Task<DragaliaResult> StartMulti(DungeonStartStartMultiRequest request)
    {
        this.logger.LogDebug("Starting dungeon for quest id {questId}", request.quest_id);

        IngameQuestData ingameQuestData = await this.dungeonStartService.InitiateQuest(
            request.quest_id
        );

        UpdateDataList updateData = await this.updateDataService.SaveChangesAsync();

        IngameData ingameData = await this.dungeonStartService.GetIngameData(
            request.quest_id,
            request.party_no_list
        );

        // TODO: Enable once co-op quest clears are fixed
        // ingameData.play_type = QuestPlayType.Multi;

        this.logger.LogDebug("Issued dungeon key {dungeonKey}", ingameData.dungeon_key);

        DungeonStartStartData response =
            new()
            {
                ingame_data = ingameData,
                ingame_quest_data = ingameQuestData,
                odds_info = StubData.OddsInfo,
                update_data_list = updateData
            };

        return this.Ok(response);
    }

    [HttpPost("start_assign_unit")]
    public async Task<DragaliaResult> StartAssignUnit(DungeonStartStartAssignUnitRequest request)
    {
        this.logger.LogDebug("Starting dungeon for quest id {questId}", request.quest_id);

        IngameQuestData ingameQuestData = await this.dungeonStartService.InitiateQuest(
            request.quest_id
        );

        UpdateDataList updateData = await updateDataService.SaveChangesAsync();

        IngameData ingameData = await this.dungeonStartService.GetIngameData(
            request.quest_id,
            request.request_party_setting_list
        );

        this.logger.LogDebug("Issued dungeon key {dungeonKey}", ingameData.dungeon_key);

        DungeonStartStartData response =
            new()
            {
                ingame_data = ingameData,
                ingame_quest_data = ingameQuestData,
                odds_info = StubData.OddsInfo,
                update_data_list = updateData
            };

        return this.Ok(response);
    }

    private static class StubData
    {
        public static OddsInfo OddsInfo { get; } =
            new()
            {
                area_index = 0,
                reaction_obj_count = 1,
                drop_obj = new List<AtgenDropObj>()
                {
                    new()
                    {
                        drop_list = new List<AtgenDropList>()
                        {
                            new()
                            {
                                type = 13,
                                id = 1001,
                                quantity = 4000,
                                place = 0
                            }
                        },
                        obj_id = 1,
                        obj_type = 2,
                        is_rare = true,
                    }
                },
                enemy = new List<AtgenEnemy>()
                {
                    new()
                    {
                        param_id = 100010106,
                        is_pop = true,
                        is_rare = true,
                        piece = 0,
                        enemy_drop_list = new List<EnemyDropList>()
                        {
                            new()
                            {
                                drop_list = new List<AtgenDropList>(),
                                coin = 10000,
                                mana = 20000,
                            }
                        },
                        enemy_idx = 0,
                    }
                },
                grade = new List<AtgenGrade>()
            };
    }
}
