using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
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
    TimeProvider dateTimeProvider,
    IDmodeService dmodeService,
    IDmodeRepository dmodeRepository
) : DragaliaControllerBase
{
    [HttpPost("get_data")]
    public async Task<DragaliaResult> GetData(CancellationToken cancellationToken)
    {
        DmodeGetDataResponse resp = new();

        resp.CurrentServerTime = dateTimeProvider.GetUtcNow();

        resp.DmodeInfo = await dmodeService.GetInfo();
        resp.DmodeCharaList = await dmodeService.GetCharaList();
        resp.DmodeExpedition = await dmodeService.GetExpedition();
        resp.DmodeDungeonInfo = await dmodeService.GetDungeonInfo();
        resp.DmodeServitorPassiveList = await dmodeService.GetServitorPassiveList();

        resp.DmodeStoryList = (
            await storyRepository.DmodeStories.ToListAsync(cancellationToken)
        ).Select(x => new DmodeStoryList(x.StoryId, x.State == StoryState.Read));

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(CancellationToken cancellationToken)
    {
        dmodeRepository.InitializeForPlayer();
        await updateDataService.SaveChangesAsync(cancellationToken);

        // Same response
        return await GetData(cancellationToken);
    }

    [HttpPost("read_story")]
    public async Task<DragaliaResult> ReadStory(
        DmodeReadStoryRequest request,
        CancellationToken cancellationToken
    )
    {
        DmodeReadStoryResponse resp = new();

        resp.DmodeStoryRewardList = await storyService.ReadStory(
            StoryTypes.DungeonMode,
            request.DmodeStoryId
        );
        resp.DuplicateEntityList = new List<AtgenDuplicateEntityList>();
        resp.EntityResult = rewardService.GetEntityResult();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("buildup_servitor_passive")]
    public async Task<DragaliaResult> BuildupServitorPassive(
        DmodeBuildupServitorPassiveRequest request,
        CancellationToken cancellationToken
    )
    {
        DmodeBuildupServitorPassiveResponse resp = new();

        resp.DmodeServitorPassiveList = await dmodeService.BuildupServitorPassive(
            request.RequestBuildupPassiveList
        );
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("expedition_start")]
    public async Task<DragaliaResult> ExpeditionStart(
        DmodeExpeditionStartRequest request,
        CancellationToken cancellationToken
    )
    {
        DmodeExpeditionStartResponse resp = new();

        resp.DmodeExpedition = await dmodeService.StartExpedition(
            request.TargetFloorNum,
            request.CharaIdList
        );
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("expedition_finish")]
    public async Task<DragaliaResult> ExpeditionFinish(CancellationToken cancellationToken)
    {
        DmodeExpeditionFinishResponse resp = new();

        (resp.DmodeExpedition, resp.DmodeIngameResult) = await dmodeService.FinishExpedition(false);

        resp.EntityResult = rewardService.GetEntityResult();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("expedition_force_finish")]
    public async Task<DragaliaResult> ExpeditionForceFinish(CancellationToken cancellationToken)
    {
        DmodeExpeditionForceFinishResponse resp = new();

        (resp.DmodeExpedition, resp.DmodeIngameResult) = await dmodeService.FinishExpedition(true);

        resp.EntityResult = rewardService.GetEntityResult();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }
}
