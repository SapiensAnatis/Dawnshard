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
        DmodeDungeonStartData resp = new();

        (resp.dmode_dungeon_state, resp.dmode_ingame_data) = await dmodeDungeonService.StartDungeon(
            request.chara_id,
            request.start_floor_num,
            request.servitor_id,
            request.bring_edit_skill_chara_id_list
        );

        await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("restart")]
    public async Task<DragaliaResult> Restart()
    {
        DmodeDungeonRestartData resp = new();

        (resp.dmode_dungeon_state, resp.dmode_ingame_data) =
            await dmodeDungeonService.RestartDungeon();

        await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("floor")]
    public async Task<DragaliaResult> Floor(DmodeDungeonFloorRequest request)
    {
        DmodeDungeonFloorData resp = new();

        (resp.dmode_dungeon_state, resp.dmode_floor_data) =
            await dmodeDungeonService.ProgressToNextFloor(request.dmode_play_record);

        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("finish")]
    public async Task<DragaliaResult> Finish(DmodeDungeonFinishRequest request)
    {
        DmodeDungeonFinishData resp = new();

        (resp.dmode_dungeon_state, resp.dmode_ingame_result) =
            await dmodeDungeonService.FinishDungeon(request.is_game_over);

        resp.entity_result = rewardService.GetEntityResult();
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("floor_skip")]
    public async Task<DragaliaResult> FloorSkip()
    {
        DmodeDungeonFloorSkipData resp = new();

        resp.dmode_dungeon_state = await dmodeDungeonService.SkipFloor();
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("user_halt")]
    public async Task<DragaliaResult> UserHalt()
    {
        DmodeDungeonUserHaltData resp = new();

        resp.dmode_dungeon_state = await dmodeDungeonService.HaltDungeon(true);
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("system_halt")]
    public async Task<DragaliaResult> SystemHalt()
    {
        DmodeDungeonSystemHaltData resp = new();

        resp.dmode_dungeon_state = await dmodeDungeonService.HaltDungeon(false);
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }
}
