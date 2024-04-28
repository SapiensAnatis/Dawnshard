using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.StorySkip;

[Route("story_skip")]
public class StorySkipController : DragaliaControllerBase
{
    private readonly ILogger<StorySkipController> logger;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly IStorySkipService storySkipService;
    private readonly IUpdateDataService updateDataService;

    public StorySkipController(
        ILogger<StorySkipController> logger,
        IPlayerIdentityService playerIdentityService,
        IStorySkipService storySkipService,
        IUpdateDataService updateDataService
    )
    {
        this.logger = logger;
        this.playerIdentityService = playerIdentityService;
        this.storySkipService = storySkipService;
        this.updateDataService = updateDataService;
    }

    [HttpPost("skip")]
    public async Task<DragaliaResult> Read(
        CancellationToken cancellationToken
    )
    {
        try
        {
            this.logger.LogDebug("Beginning story skip for player.");

            IEnumerable<QuestData> questDatas = MasterAsset.QuestData.Enumerable.Where(x =>
                x.Gid < 10011 && x.Id > 100000000 && x.Id.ToString().Substring(6, 1) == "1"
            );
            IEnumerable<QuestTreasureData> questTreasureDatas = MasterAsset.QuestTreasureData.Enumerable.Where(x =>
                x.Id is < 110000 && x.Id.ToString().Substring(3, 1) == "1"
            );
            IEnumerable<QuestStory> questStories = MasterAsset.QuestStory.Enumerable.Where(
                x => x.GroupId is < 10011
            );

            foreach (QuestData questData in questDatas)
            {
                await storySkipService.ProcessQuestCompletion(questData.Id);
            }

            foreach (QuestTreasureData questTreasureData in questTreasureDatas)
            {
                await storySkipService.OpenTreasure(questTreasureData.Id, cancellationToken);
            }

            foreach (QuestStory questStory in questStories)
            {
                await storySkipService.ReadStory(questStory.Id, cancellationToken);
            }

            await storySkipService.IncreasePlayerLevel();

            await storySkipService.IncreaseFortLevels();

            UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

            this.logger.LogDebug("Story Skip completed for player {AccountId}.", playerIdentityService.AccountId);

            return this.Ok(
                new StorySkipSkipResponse() { ResultState = 1 }
            );
        }
        catch (Exception e)
        {
            this.logger.LogError("Error occurred while skipping story: {Message}", e.Message);
            return this.Ok(
                new StorySkipSkipResponse() { ResultState = 0 }
            );
        }
    }
}
