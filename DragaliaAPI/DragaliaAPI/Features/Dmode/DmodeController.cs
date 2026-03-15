using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Features.Story;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
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
        DmodeGetDataResponse resp = new()
        {
            CurrentServerTime = dateTimeProvider.GetUtcNow(),
            DmodeInfo = await dmodeService.GetInfo(),
            DmodeCharaList = await dmodeService.GetCharaList(),
            DmodeExpedition = await dmodeService.GetExpedition(),
            DmodeDungeonInfo = await dmodeService.GetDungeonInfo(),
            DmodeServitorPassiveList = await dmodeService.GetServitorPassiveList(),
            DmodeStoryList = (
                await storyRepository.DmodeStories.ToListAsync(cancellationToken)
            ).Select(x => new DmodeStoryList(x.StoryId, x.State == StoryState.Read)),
            UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken),
        };

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
        DmodeReadStoryResponse resp = new()
        {
            DmodeStoryRewardList = await storyService.ReadStory(
                StoryTypes.DungeonMode,
                request.DmodeStoryId
            ),
            DuplicateEntityList = new List<AtgenDuplicateEntityList>(),
            EntityResult = rewardService.GetEntityResult(),
            UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken),
        };

        return Ok(resp);
    }

    [HttpPost("buildup_servitor_passive")]
    public async Task<DragaliaResult> BuildupServitorPassive(
        DmodeBuildupServitorPassiveRequest request,
        CancellationToken cancellationToken
    )
    {
        DmodeBuildupServitorPassiveResponse resp = new()
        {
            DmodeServitorPassiveList = await dmodeService.BuildupServitorPassive(
                request.RequestBuildupPassiveList
            ),
            UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken),
        };

        return Ok(resp);
    }

    [HttpPost("expedition_start")]
    public async Task<DragaliaResult> ExpeditionStart(
        DmodeExpeditionStartRequest request,
        CancellationToken cancellationToken
    )
    {
        DmodeExpeditionStartResponse resp = new()
        {
            DmodeExpedition = await dmodeService.StartExpedition(
                request.TargetFloorNum,
                request.CharaIdList
            ),
            UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken),
        };

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
