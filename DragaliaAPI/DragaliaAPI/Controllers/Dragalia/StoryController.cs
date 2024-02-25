using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("story")]
public class StoryController : DragaliaControllerBase
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
    public async Task<DragaliaResult> Read(StoryReadRequest request)
    {
        if (!MasterAsset.UnitStory.TryGetValue(request.UnitStoryId, out UnitStory? data))
        {
            this.logger.LogWarning(
                "Requested to read non-existent unit story {id}",
                request.UnitStoryId
            );

            return this.Code(ResultCode.StoryNotGet);
        }

        if (!await this.storyService.CheckStoryEligibility(data.Type, request.UnitStoryId))
        {
            this.logger.LogWarning("User did not have access to story {id}", request.UnitStoryId);
            return this.Code(ResultCode.StoryNotReadThePrevious);
        }

        IEnumerable<AtgenBuildEventRewardEntityList> rewards = await this.storyService.ReadStory(
            data.Type,
            request.UnitStoryId
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(
            new StoryReadData() { UnitStoryRewardList = rewards, UpdateDataList = updateDataList, }
        );
    }
}
