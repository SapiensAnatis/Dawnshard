using AutoMapper;
using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Wall;

[Route("wall_start")]
public class WallStartController : DragaliaControllerBase
{
    private readonly IMapper mapper;
    private readonly ILogger<WallStartController> logger;
    private readonly IUpdateDataService updateDataService;
    private readonly IOddsInfoService oddsInfoService;
    private readonly IWallService wallService;
    private readonly IDungeonStartService dungeonStartService;

    public WallStartController(
        IMapper mapper,
        ILogger<WallStartController> logger,
        IUpdateDataService updateDataService,
        IOddsInfoService oddsInfoService,
        IWallService wallService,
        IDungeonStartService dungeonStartService
    )
    {
        this.mapper = mapper;
        this.logger = logger;
        this.updateDataService = updateDataService;
        this.oddsInfoService = oddsInfoService;
        this.wallService = wallService;
        this.dungeonStartService = dungeonStartService;
    }

    // Called when starting a Mercurial Gauntlet quest
    [HttpPost("start")]
    public async Task<DragaliaResult> Start(WallStartStartRequest request)
    {
        // Set flag for having played the next level
        await wallService.SetQuestWallIsStartNextLevel(request.wall_id, true);
        QuestWallDetail questWallDetail = MasterAssetUtils.GetQuestWallDetail(
            request.wall_id,
            request.wall_level
        );

        IngameData ingameData = await this.dungeonStartService.GetWallIngameData(
            request.wall_id,
            request.wall_level,
            request.party_no,
            request.support_viewer_id
        );

        ingameData.area_info_list = questWallDetail.AreaInfo.Select(mapper.Map<AreaInfoList>);

        IngameWallData ingameWallData =
            new() { wall_id = request.wall_id, wall_level = request.wall_level };

        OddsInfo oddsInfo = this.oddsInfoService.GetWallOddsInfo(
            request.wall_id,
            request.wall_level
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        WallStartStartData data =
            new()
            {
                ingame_data = ingameData,
                ingame_wall_data = ingameWallData,
                odds_info = oddsInfo,
                update_data_list = updateDataList
            };

        return Ok(data);
    }

    // Called from the play next level button from the MG clear screen
    [HttpPost("start_assign_unit")]
    public async Task<DragaliaResult> StartAssignUnit(WallStartStartAssignUnitRequest request)
    {
        QuestWallDetail questWallDetail = MasterAssetUtils.GetQuestWallDetail(
            request.wall_id,
            request.wall_level
        );

        IngameData ingameData = await this.dungeonStartService.GetWallIngameData(
            request.wall_id,
            request.wall_level,
            request.request_party_setting_list,
            request.support_viewer_id
        );

        ingameData.area_info_list = questWallDetail.AreaInfo.Select(mapper.Map<AreaInfoList>);

        IngameWallData ingameWallData =
            new() { wall_id = request.wall_id, wall_level = request.wall_level };

        OddsInfo oddsInfo = this.oddsInfoService.GetWallOddsInfo(
            request.wall_id,
            request.wall_level
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        WallStartStartAssignUnitData data =
            new()
            {
                ingame_data = ingameData,
                ingame_wall_data = ingameWallData,
                odds_info = oddsInfo,
                update_data_list = updateDataList
            };

        return Ok(data);
    }
}
