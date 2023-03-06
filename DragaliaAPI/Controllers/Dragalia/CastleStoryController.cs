using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            !await this.storyService.CheckStoryEligibility(
                StoryTypes.Castle,
                request.castle_story_id
            )
        )
        {
            this.logger.LogWarning(
                "User was not eligible to read castle story {id}",
                request.castle_story_id
            );
            return this.Code(ResultCode.StoryNotGet);
        }

        IEnumerable<AtgenBuildEventRewardEntityList> rewardList = await this.storyService.ReadStory(
            StoryTypes.Castle,
            request.castle_story_id
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(
            new CastleStoryReadData()
            {
                castle_story_reward_list = rewardList,
                update_data_list = updateDataList
            }
        );
    }
}
