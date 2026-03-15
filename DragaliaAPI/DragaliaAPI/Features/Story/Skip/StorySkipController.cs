using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Story.Skip;

[Route("story_skip")]
public partial class StorySkipController(
    ILogger<StorySkipController> logger,
    StorySkipService storySkipService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost("skip")]
    public async Task<DragaliaResult> Read(CancellationToken cancellationToken)
    {
        Log.BeginningStorySkip(logger);

        int wyrmite1 = await storySkipService.ProcessQuestCompletions();
        Log.WyrmiteEarnedFromQuests(logger, wyrmite1);

        int wyrmite2 = await storySkipService.ProcessStoryCompletions();
        Log.WyrmiteEarnedFromQuestStories(logger, wyrmite2);

        await storySkipService.UpdateUserData(wyrmite1 + wyrmite2);

        await storySkipService.IncreaseFortLevels();

        await storySkipService.RewardCharas();

        await storySkipService.RewardDragons();

        await storySkipService.InitializeWall();

        await updateDataService.SaveChangesAsync(cancellationToken);

        Log.StorySkipCompleted(logger);

        return this.Ok(new StorySkipSkipResponse() { ResultState = 1 });
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Beginning story skip.")]
        public static partial void BeginningStorySkip(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Wyrmite earned from quests: {Wyrmite}")]
        public static partial void WyrmiteEarnedFromQuests(ILogger logger, int wyrmite);

        [LoggerMessage(LogLevel.Debug, "Wyrmite earned from quest stories: {Wyrmite}")]
        public static partial void WyrmiteEarnedFromQuestStories(ILogger logger, int wyrmite);

        [LoggerMessage(LogLevel.Debug, "Story Skip completed.")]
        public static partial void StorySkipCompleted(ILogger logger);
    }
}
