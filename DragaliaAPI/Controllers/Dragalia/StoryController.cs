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
        if (!MasterAsset.UnitStory.TryGetValue(request.unit_story_id, out UnitStory? data))
        {
            this.logger.LogWarning(
                "Requested to read non-existent unit story {id}",
                request.unit_story_id
            );

            return this.Code(ResultCode.StoryNotGet);
        }

        if (!await this.storyService.CheckStoryEligibility(data.Type, request.unit_story_id))
        {
            this.logger.LogWarning("User did not have access to story {id}", request.unit_story_id);
            return this.Code(ResultCode.StoryNotReadThePrevious);
        }

        IEnumerable<AtgenBuildEventRewardEntityList> rewards = await this.storyService.ReadStory(
            data.Type,
            request.unit_story_id
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(
            new StoryReadData()
            {
                unit_story_reward_list = rewards,
                update_data_list = updateDataList,
            }
        );
    }
}
