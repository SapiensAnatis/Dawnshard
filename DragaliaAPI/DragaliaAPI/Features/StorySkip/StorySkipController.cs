using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.StorySkip;

[Route("story_skip")]
public class StorySkipController : DragaliaControllerBase
{
    private readonly ILogger<StorySkipController> logger;
    private readonly IStorySkipService storySkipService;
    private readonly IUpdateDataService updateDataService;

    public StorySkipController(
        ILogger<StorySkipController> logger,
        IStorySkipService storySkipService,
        IUpdateDataService updateDataService
    )
    {
        this.logger = logger;
        this.storySkipService = storySkipService;
        this.updateDataService = updateDataService;
    }

    [HttpPost("skip")]
    public async Task<DragaliaResult> Read(
        CancellationToken cancellationToken
    )
    {
        this.logger.LogDebug("Beginning story skip for player.");

        List<int> storyQuestIds = StorySkipLists.StoryQuestIds;
        List<int> questIds = StorySkipLists.QuestIds;

        foreach (int storyQuestId in storyQuestIds)
        {
            await storySkipService.ReadStory(storyQuestId, cancellationToken);
        }

        foreach (int questId in questIds)
        {
            await storySkipService.ProcessQuestCompletion(questId);
        }

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        this.logger.LogDebug("Story Skip completed for player {Name}.", updateDataList.UserData.Name);

        return this.Ok(
            new StorySkipSkipResponse() { }
        );
    }
}
