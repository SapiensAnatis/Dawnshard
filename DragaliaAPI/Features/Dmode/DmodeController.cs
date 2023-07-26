using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dmode;

[Route("dmode")]
public class DmodeController(
    IUpdateDataService updateDataService,
    IStoryRepository storyRepository,
    IRewardService rewardService,
    IStoryService storyService,
    IDateTimeProvider dateTimeProvider,
    IDmodeService dmodeService,
    IDmodeRepository dmodeRepository
) : DragaliaControllerBase
{
    [HttpPost("get_data")]
    public async Task<DragaliaResult> GetData()
    {
        DmodeGetDataData resp = new();

        resp.current_server_time = dateTimeProvider.UtcNow;

        resp.dmode_info = await dmodeService.GetInfo();
        resp.dmode_chara_list = await dmodeService.GetCharaList();
        resp.dmode_expedition = await dmodeService.GetExpedition();
        resp.dmode_dungeon_info = await dmodeService.GetDungeonInfo();
        resp.dmode_servitor_passive_list = await dmodeService.GetServitorPassiveList();

        resp.dmode_story_list = (await storyRepository.DmodeStories.ToListAsync()).Select(
            x => new DmodeStoryList(x.StoryId, (int)x.State)
        );

        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry()
    {
        dmodeRepository.InitializeForPlayer();
        await updateDataService.SaveChangesAsync();

        // Same response
        return await GetData();
    }

    [HttpPost("read_story")]
    public async Task<DragaliaResult> ReadStory(DmodeReadStoryRequest request)
    {
        DmodeReadStoryData resp = new();

        resp.dmode_story_reward_list = await storyService.ReadStory(
            StoryTypes.DungeonMode,
            request.dmode_story_id
        );
        resp.duplicate_entity_list = new List<AtgenDuplicateEntityList>();
        resp.entity_result = rewardService.GetEntityResult();
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("buildup_servitor_passive")]
    public async Task<DragaliaResult> BuildupServitorPassive(
        DmodeBuildupServitorPassiveRequest request
    )
    {
        DmodeBuildupServitorPassiveData resp = new();

        resp.dmode_servitor_passive_list = await dmodeService.BuildupServitorPassive(
            request.request_buildup_passive_list
        );
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("expedition_start")]
    public async Task<DragaliaResult> ExpeditionStart(DmodeExpeditionStartRequest request)
    {
        DmodeExpeditionStartData resp = new();

        resp.dmode_expedition = await dmodeService.StartExpedition(
            request.target_floor_num,
            request.chara_id_list
        );
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("expedition_finish")]
    public async Task<DragaliaResult> ExpeditionFinish()
    {
        DmodeExpeditionFinishData resp = new();

        (resp.dmode_expedition, resp.dmode_ingame_result) = await dmodeService.FinishExpedition(
            false
        );

        resp.entity_result = rewardService.GetEntityResult();
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("expedition_force_finish")]
    public async Task<DragaliaResult> ExpeditionForceFinish()
    {
        DmodeExpeditionForceFinishData resp = new();

        (resp.dmode_expedition, resp.dmode_ingame_result) = await dmodeService.FinishExpedition(
            true
        );

        resp.entity_result = rewardService.GetEntityResult();
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }
}
