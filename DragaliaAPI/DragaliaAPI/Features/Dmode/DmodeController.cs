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

        resp.CurrentServerTime = dateTimeProvider.UtcNow;

        resp.DmodeInfo = await dmodeService.GetInfo();
        resp.DmodeCharaList = await dmodeService.GetCharaList();
        resp.DmodeExpedition = await dmodeService.GetExpedition();
        resp.DmodeDungeonInfo = await dmodeService.GetDungeonInfo();
        resp.DmodeServitorPassiveList = await dmodeService.GetServitorPassiveList();

        resp.DmodeStoryList = (await storyRepository.DmodeStories.ToListAsync()).Select(
            x => new DmodeStoryList(x.StoryId, (int)x.State)
        );

        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

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

        resp.DmodeStoryRewardList = await storyService.ReadStory(
            StoryTypes.DungeonMode,
            request.DmodeStoryId
        );
        resp.DuplicateEntityList = new List<AtgenDuplicateEntityList>();
        resp.EntityResult = rewardService.GetEntityResult();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("buildup_servitor_passive")]
    public async Task<DragaliaResult> BuildupServitorPassive(
        DmodeBuildupServitorPassiveRequest request
    )
    {
        DmodeBuildupServitorPassiveData resp = new();

        resp.DmodeServitorPassiveList = await dmodeService.BuildupServitorPassive(
            request.RequestBuildupPassiveList
        );
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("expedition_start")]
    public async Task<DragaliaResult> ExpeditionStart(DmodeExpeditionStartRequest request)
    {
        DmodeExpeditionStartData resp = new();

        resp.DmodeExpedition = await dmodeService.StartExpedition(
            request.TargetFloorNum,
            request.CharaIdList
        );
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("expedition_finish")]
    public async Task<DragaliaResult> ExpeditionFinish()
    {
        DmodeExpeditionFinishData resp = new();

        (resp.DmodeExpedition, resp.DmodeIngameResult) = await dmodeService.FinishExpedition(false);

        resp.EntityResult = rewardService.GetEntityResult();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("expedition_force_finish")]
    public async Task<DragaliaResult> ExpeditionForceFinish()
    {
        DmodeExpeditionForceFinishData resp = new();

        (resp.DmodeExpedition, resp.DmodeIngameResult) = await dmodeService.FinishExpedition(true);

        resp.EntityResult = rewardService.GetEntityResult();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }
}
