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
public class WallStartController(
    IMapper mapper,
    IUpdateDataService updateDataService,
    IOddsInfoService oddsInfoService,
    IWallService wallService,
    IDungeonStartService dungeonStartService
) : DragaliaControllerBase
{
    private readonly IMapper mapper = mapper;
    private readonly IUpdateDataService updateDataService = updateDataService;
    private readonly IOddsInfoService oddsInfoService = oddsInfoService;
    private readonly IWallService wallService = wallService;
    private readonly IDungeonStartService dungeonStartService = dungeonStartService;

    // Called when starting a Mercurial Gauntlet quest
    [HttpPost("start")]
    public async Task<DragaliaResult> Start(
        WallStartStartRequest request,
        CancellationToken cancellationToken
    )
    {
        // Set flag for having played the next level
        await wallService.SetQuestWallIsStartNextLevel(request.WallId, true);
        QuestWallDetail questWallDetail = MasterAssetUtils.GetQuestWallDetail(
            request.WallId,
            request.WallLevel
        );

        IngameData ingameData = await this.dungeonStartService.GetWallIngameData(
            request.WallId,
            request.WallLevel,
            request.PartyNo,
            request.SupportViewerId
        );

        ingameData.AreaInfoList = questWallDetail.AreaInfo.Select(mapper.Map<AreaInfoList>);

        IngameWallData ingameWallData =
            new() { WallId = request.WallId, WallLevel = request.WallLevel };

        OddsInfo oddsInfo = this.oddsInfoService.GetWallOddsInfo(request.WallId, request.WallLevel);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        WallStartStartResponse data =
            new()
            {
                IngameData = ingameData,
                IngameWallData = ingameWallData,
                OddsInfo = oddsInfo,
                UpdateDataList = updateDataList
            };

        return Ok(data);
    }

    // Called from the play next level button from the MG clear screen
    [HttpPost("start_assign_unit")]
    public async Task<DragaliaResult> StartAssignUnit(
        WallStartStartAssignUnitRequest request,
        CancellationToken cancellationToken
    )
    {
        QuestWallDetail questWallDetail = MasterAssetUtils.GetQuestWallDetail(
            request.WallId,
            request.WallLevel
        );

        IngameData ingameData = await this.dungeonStartService.GetWallIngameData(
            request.WallId,
            request.WallLevel,
            request.RequestPartySettingList,
            request.SupportViewerId
        );

        ingameData.AreaInfoList = questWallDetail.AreaInfo.Select(mapper.Map<AreaInfoList>);

        IngameWallData ingameWallData =
            new() { WallId = request.WallId, WallLevel = request.WallLevel };

        OddsInfo oddsInfo = this.oddsInfoService.GetWallOddsInfo(request.WallId, request.WallLevel);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        WallStartStartAssignUnitResponse data =
            new()
            {
                IngameData = ingameData,
                IngameWallData = ingameWallData,
                OddsInfo = oddsInfo,
                UpdateDataList = updateDataList
            };

        return Ok(data);
    }
}
