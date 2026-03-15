using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Story;

[Route("story")]
public partial class StoryController : DragaliaControllerBase
{
    private readonly IStoryService storyService;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<StoryController> logger;

    public StoryController(
        IStoryService storyService,
        IUpdateDataService updateDataService,
        ILogger<StoryController> logger
    )
    {
        this.storyService = storyService;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost("read")]
    public async Task<DragaliaResult> Read(
        StoryReadRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!MasterAsset.UnitStory.TryGetValue(request.UnitStoryId, out UnitStory? data))
        {
            Log.RequestedToReadNonExistentUnitStory(this.logger, request.UnitStoryId);

            return this.Code(ResultCode.StoryNotGet);
        }

        if (!await this.storyService.CheckStoryEligibility(data.Type, request.UnitStoryId))
        {
            Log.UserDidNotHaveAccessToStory(this.logger, request.UnitStoryId);
            return this.Code(ResultCode.StoryNotReadThePrevious);
        }

        IEnumerable<AtgenBuildEventRewardEntityList> rewards = await this.storyService.ReadStory(
            data.Type,
            request.UnitStoryId
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        return this.Ok(
            new StoryReadResponse()
            {
                UnitStoryRewardList = rewards,
                UpdateDataList = updateDataList,
            }
        );
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Warning, "Requested to read non-existent unit story {id}")]
        public static partial void RequestedToReadNonExistentUnitStory(ILogger logger, int id);

        [LoggerMessage(LogLevel.Warning, "User did not have access to story {id}")]
        public static partial void UserDidNotHaveAccessToStory(ILogger logger, int id);
    }
}
