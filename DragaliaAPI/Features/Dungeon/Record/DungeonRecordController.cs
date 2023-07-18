using System.Diagnostics;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon.Record;

[Route("dungeon_record")]
public class DungeonRecordController : DragaliaControllerBase
{
    private readonly IUpdateDataService updateDataService;
    private readonly IDungeonRecordService dungeonRecordService;
    private readonly ITutorialService tutorialService;
    private readonly ILogger<DungeonRecordController> logger;

    public DungeonRecordController(
        IUpdateDataService updateDataService,
        IDungeonRecordService dungeonRecordService,
        ITutorialService tutorialService,
        ILogger<DungeonRecordController> logger
    )
    {
        this.updateDataService = updateDataService;
        this.dungeonRecordService = dungeonRecordService;
        this.tutorialService = tutorialService;
        this.logger = logger;
    }

    [HttpPost("record")]
    public async Task<DragaliaResult> Record(DungeonRecordRecordRequest request)
    {
        await tutorialService.AddTutorialFlag(1022);

        IngameResultData ingameResultData =
            await this.dungeonRecordService.GenerateIngameResultData(
                request.dungeon_key,
                request.play_record
            );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        DungeonRecordRecordData response =
            new() { ingame_result_data = ingameResultData, update_data_list = updateDataList, };

        return Ok(response);
    }

    [HttpPost("record_multi")]
    [Authorize(AuthenticationSchemes = nameof(PhotonAuthenticationHandler))]
    public async Task<DragaliaResult> RecordMulti(DungeonRecordRecordMultiRequest request)
    {
        await tutorialService.AddTutorialFlag(1022);

        IngameResultData ingameResultData =
            await this.dungeonRecordService.GenerateIngameResultData(
                request.dungeon_key,
                request.play_record
            );

        ingameResultData.play_type = QuestPlayType.Multi;

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        DungeonRecordRecordMultiData response =
            new() { ingame_result_data = ingameResultData, update_data_list = updateDataList, };

        return Ok(response);
    }
}
