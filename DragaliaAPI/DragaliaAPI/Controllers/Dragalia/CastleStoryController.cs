using DragaliaAPI.DTO;
using DragaliaAPI.Features.ClientState;
using DragaliaAPI.Features.Savefile;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("castle_story")]
public class CastleStoryController(
    IStoryService storyService,
    IUpdateDataService updateDataService,
    ILogger<CastleStoryController> logger
) : DragaliaControllerBase
{
    private readonly IStoryService storyService = storyService;
    private readonly IUpdateDataService updateDataService = updateDataService;
    private readonly ILogger<CastleStoryController> logger = logger;

    [HttpPost("read")]
    public async Task<DragaliaResult> Read(
        CastleStoryReadRequest request,
        CancellationToken cancellationToken
    )
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

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        return this.Ok(
            new CastleStoryReadResponse()
            {
                CastleStoryRewardList = rewardList,
                UpdateDataList = updateDataList
            }
        );
    }
}
