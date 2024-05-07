using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.StorySkip;

[Route("story_skip")]
public class StorySkipController : DragaliaControllerBase
{
    private readonly ILogger<StorySkipController> logger;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly IQuestRepository questRepository;
    private readonly StorySkipService storySkipService;
    private readonly IUpdateDataService updateDataService;
    private readonly IUserDataRepository userDataRepository;

    public StorySkipController(
        ILogger<StorySkipController> logger,
        IPlayerIdentityService playerIdentityService,
        IQuestRepository questRepository,
        StorySkipService storySkipService,
        IUpdateDataService updateDataService,
        IUserDataRepository userDataRepository
    )
    {
        this.logger = logger;
        this.playerIdentityService = playerIdentityService;
        this.questRepository = questRepository;
        this.storySkipService = storySkipService;
        this.updateDataService = updateDataService;
        this.userDataRepository = userDataRepository;
    }

    [HttpPost("skip")]
    public async Task<DragaliaResult> Read(CancellationToken cancellationToken)
    {
        string accountId = playerIdentityService.AccountId;
        long viewerId = playerIdentityService.ViewerId;

        this.logger.LogDebug("Beginning story skip for player {accountId}.", accountId);

        int wyrmite1 = await storySkipService.ProcessQuestCompletions(viewerId);
        this.logger.LogDebug("Wyrmite earned from quests: {wyrmite}", wyrmite1);

        int wyrmite2 = await storySkipService.ProcessStoryCompletions(viewerId);
        this.logger.LogDebug("Wyrmite earned from quest stories: {wyrmite}", wyrmite2);

        await storySkipService.UpdateUserData(wyrmite1 + wyrmite2);

        await storySkipService.IncreaseFortLevels(viewerId);

        await storySkipService.RewardCharas(viewerId);

        await storySkipService.RewardDragons(viewerId);

        await updateDataService.SaveChangesAsync(cancellationToken);

        this.logger.LogDebug("Story Skip completed for player {accountId}.", accountId);

        return this.Ok(new StorySkipSkipResponse() { ResultState = 1 });
    }
}
