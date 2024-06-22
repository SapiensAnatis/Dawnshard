using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.StorySkip;

[Route("story_skip")]
public class StorySkipController(
    ILogger<StorySkipController> logger,
    StorySkipService storySkipService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost("skip")]
    public async Task<DragaliaResult> Read(CancellationToken cancellationToken)
    {
        logger.LogDebug("Beginning story skip.");

        int wyrmite1 = await storySkipService.ProcessQuestCompletions();
        logger.LogDebug("Wyrmite earned from quests: {Wyrmite}", wyrmite1);

        int wyrmite2 = await storySkipService.ProcessStoryCompletions();
        logger.LogDebug("Wyrmite earned from quest stories: {Wyrmite}", wyrmite2);

        await storySkipService.UpdateUserData(wyrmite1 + wyrmite2);

        await storySkipService.IncreaseFortLevels();

        await storySkipService.RewardCharas();

        await storySkipService.RewardDragons();

        await storySkipService.InitializeWall();

        await updateDataService.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Story Skip completed.");

        return this.Ok(new StorySkipSkipResponse() { ResultState = 1 });
    }
}
