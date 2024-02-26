﻿using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("castle_story")]
public class CastleStoryController : DragaliaControllerBase
{
    private readonly IStoryService storyService;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<CastleStoryController> logger;

    public CastleStoryController(
        IStoryService storyService,
        IUpdateDataService updateDataService,
        ILogger<CastleStoryController> logger
    )
    {
        this.storyService = storyService;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost("read")]
    public async Task<DragaliaResult> Read(CastleStoryReadRequest request)
    {
        if (
            !await this.storyService.CheckStoryEligibility(StoryTypes.Castle, request.CastleStoryId)
        )
        {
            this.logger.LogWarning(
                "User was not eligible to read castle story {id}",
                request.CastleStoryId
            );
            return this.Code(ResultCode.StoryNotGet);
        }

        IEnumerable<AtgenBuildEventRewardEntityList> rewardList = await this.storyService.ReadStory(
            StoryTypes.Castle,
            request.CastleStoryId
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(
            new CastleStoryReadResponse()
            {
                CastleStoryRewardList = rewardList,
                UpdateDataList = updateDataList
            }
        );
    }
}
