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
    private readonly IDungeonService dungeonService;
    private readonly IOddsInfoService oddsInfoService;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<DungeonStartController> logger;

    public DungeonStartController(
        IDungeonStartService dungeonStartService,
        IDungeonService dungeonService,
        IOddsInfoService oddsInfoService,
        IUpdateDataService updateDataService,
        ILogger<DungeonStartController> logger
    )
    {
        this.dungeonStartService = dungeonStartService;
        this.dungeonService = dungeonService;
        this.oddsInfoService = oddsInfoService;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost("start")]
    public async Task<DragaliaResult> Start(DungeonStartStartRequest request)
    {
        IngameData ingameData = await this.dungeonStartService.GetIngameData(
            request.quest_id,
            request.party_no_list,
            request.support_viewer_id
        );

        DungeonStartStartData response = await this.BuildResponse(request.quest_id, ingameData);

        return this.Ok(response);
    }

    [HttpPost("start_multi")]
    public async Task<DragaliaResult> StartMulti(DungeonStartStartMultiRequest request)
    {
        IngameData ingameData = await this.dungeonStartService.GetIngameData(
            request.quest_id,
            request.party_no_list
        );

        DungeonStartStartData response = await this.BuildResponse(request.quest_id, ingameData);

        // TODO: Enable when co-op is fixed
        // response.ingame_data.play_type = QuestPlayType.Multi;

        return this.Ok(response);
    }

    [HttpPost("start_assign_unit")]
    public async Task<DragaliaResult> StartAssignUnit(DungeonStartStartAssignUnitRequest request)
    {
        IngameData ingameData = await this.dungeonStartService.GetIngameData(
            request.quest_id,
            request.request_party_setting_list,
            request.support_viewer_id
        );

        DungeonStartStartData response = await this.BuildResponse(request.quest_id, ingameData);

        return this.Ok(response);
    }

    private async Task<DungeonStartStartData> BuildResponse(int questId, IngameData ingameData)
    {
        this.logger.LogDebug("Starting dungeon for quest id {questId}", questId);

        IngameQuestData ingameQuestData = await this.dungeonStartService.InitiateQuest(questId);

        UpdateDataList updateData = await updateDataService.SaveChangesAsync();

        OddsInfo oddsInfo = this.oddsInfoService.GetOddsInfo(questId, 0);
        await this.dungeonService.AddEnemies(ingameData.dungeon_key, 0, oddsInfo.enemy);

        return new()
        {
            ingame_data = ingameData,
            ingame_quest_data = ingameQuestData,
            odds_info = oddsInfo,
            update_data_list = updateData
        };
    }
}
