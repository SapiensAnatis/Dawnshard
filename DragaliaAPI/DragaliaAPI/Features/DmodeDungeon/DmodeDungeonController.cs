using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.DmodeDungeon;

[Route("dmode_dungeon")]
public class DmodeDungeonController(
    IDmodeDungeonService dmodeDungeonService,
    IUpdateDataService updateDataService,
    IRewardService rewardService
) : DragaliaControllerBase
{
    [HttpPost("start")]
    public async Task<DragaliaResult> Start(DmodeDungeonStartRequest request)
    {
        DmodeDungeonStartResponse resp = new();

        (resp.DmodeDungeonState, resp.DmodeIngameData) = await dmodeDungeonService.StartDungeon(
            request.CharaId,
            request.StartFloorNum,
            request.ServitorId,
            request.BringEditSkillCharaIdList
        );

        await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("restart")]
    public async Task<DragaliaResult> Restart()
    {
        DmodeDungeonRestartResponse resp = new();

        (resp.DmodeDungeonState, resp.DmodeIngameData) = await dmodeDungeonService.RestartDungeon();

        await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("floor")]
    public async Task<DragaliaResult> Floor(DmodeDungeonFloorRequest request)
    {
        DmodeDungeonFloorResponse resp = new();

        (resp.DmodeDungeonState, resp.DmodeFloorData) =
            await dmodeDungeonService.ProgressToNextFloor(request.DmodePlayRecord);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("finish")]
    public async Task<DragaliaResult> Finish(DmodeDungeonFinishRequest request)
    {
        DmodeDungeonFinishResponse resp = new();

        (resp.DmodeDungeonState, resp.DmodeIngameResult) = await dmodeDungeonService.FinishDungeon(
            request.IsGameOver
        );

        resp.EntityResult = rewardService.GetEntityResult();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("floor_skip")]
    public async Task<DragaliaResult> FloorSkip()
    {
        DmodeDungeonFloorSkipResponse resp = new();

        resp.DmodeDungeonState = await dmodeDungeonService.SkipFloor();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("user_halt")]
    public async Task<DragaliaResult> UserHalt()
    {
        DmodeDungeonUserHaltResponse resp = new();

        resp.DmodeDungeonState = await dmodeDungeonService.HaltDungeon(true);
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("system_halt")]
    public async Task<DragaliaResult> SystemHalt()
    {
        DmodeDungeonSystemHaltResponse resp = new();

        resp.DmodeDungeonState = await dmodeDungeonService.HaltDungeon(false);
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }
}
